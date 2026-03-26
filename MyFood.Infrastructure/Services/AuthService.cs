using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyFood.Domain;

namespace MyFood.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<(bool Success, string? Token, string? Error)> RegisterAsync(string username, string fullName, string password)
    {
        var normalizedUsername = username.Trim();

        var existingUser = await _userManager.FindByNameAsync(normalizedUsername);
        if (existingUser is not null)
        {
            return (false, null, "User already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = normalizedUsername,
            FullName = fullName.Trim()
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, null, $"Registration failed: {errors}");
        }

        var token = GenerateJwtToken(user);
        return (true, token, null);
    }

    public async Task<(bool Success, string? Token, string? Error)> LoginAsync(string username, string password)
    {
        var normalizedUsername = username.Trim();

        var user = await _userManager.FindByNameAsync(normalizedUsername);
        if (user is null)
        {
            return (false, null, "Invalid credentials.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordValid)
        {
            return (false, null, "Invalid credentials.");
        }

        var token = GenerateJwtToken(user);
        return (true, token, null);
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim("UserId", user.Id),
            new Claim("Username", user.UserName ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty)
        };

        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key configuration is missing. Please check appsettings.json");
        var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer configuration is missing. Please check appsettings.json");
        var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience configuration is missing. Please check appsettings.json");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var durationMinutes = 60;
        if (_configuration["Jwt:DurationInMinutes"] != null &&
            double.TryParse(_configuration["Jwt:DurationInMinutes"], out var parsedDuration))
        {
            durationMinutes = (int)parsedDuration;
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(durationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}