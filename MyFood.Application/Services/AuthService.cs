// MyFood.Application/Services/AuthService.cs
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFood.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IConfiguration configuration,
    IPasswordService passwordService,
    IVerificationTokenService verificationTokenService) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IConfiguration _configuration = configuration;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly IVerificationTokenService _verificationTokenService = verificationTokenService;

    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto)
    {
        var username = registerDto.FullName?.Trim();
        var email = registerDto.Email?.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(registerDto.Password))
        {
            return Failure("FullName, Email, and Password are required.", 400);
        }

        if (!_passwordService.IsPasswordStrong(registerDto.Password, out var strengthError))
        {
            return Failure(strengthError, 400);
        }

        if (await _userRepository.UserExistsByUsernameAsync(username))
        {
            return Failure("User already exists.", 409);
        }

        if (await _userRepository.UserExistsByEmailAsync(email))
        {
            return Failure("Email address is already in use.", 409);
        }

        var user = new ApplicationUser
        {
            FullName = username,
            UserName = username,
            Email = email,
            IsEmailVerified = true,
            VerificationToken = null,
            VerificationTokenExpiry = null,
            PasswordHash = _passwordService.HashPassword(registerDto.Password)
        };

        try
        {
            await _userRepository.CreateUserAsync(user);
        }
        catch (Exception ex)
        {
            return Failure($"Registration failed: {ex.Message}", 400);
        }

        var token = GenerateJwtToken(user);

return new AuthResponseDto
{
    Success = true,
    StatusCode = 201,
    Message = "Registration successful.",
    Token = token,
    User = MapUser(user)
};

        return new AuthResponseDto
        {
            Success = true,
            StatusCode = 201,
            Message = "Registration successful.",
            User = MapUser(user)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var username = loginDto.FullName?.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(loginDto.Password))
        {
            return Failure("FullName and Password are required.", 400);
        }

        var user = await _userRepository.GetUserByUsernameAsync(username);
        if (user is null)
        {
            return Failure("Invalid credentials.", 401);
        }

        var isValid = _passwordService.VerifyPassword(loginDto.Password, user.PasswordHash);
        if (!isValid)
        {
            return Failure("Invalid credentials.", 401);
        }

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Success = true,
            StatusCode = 200,
            Message = "Login successful.",
            Token = token,
            User = MapUser(user)
        };
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key must be configured.");
        var key = Encoding.ASCII.GetBytes(jwtKey);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static AuthResponseDto Failure(string message, int statusCode)
    {
        return new AuthResponseDto
        {
            Success = false,
            StatusCode = statusCode,
            Message = message
        };
    }

    private static UserDto MapUser(ApplicationUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.FullName,
            Email = user.Email,
        };
    }
}