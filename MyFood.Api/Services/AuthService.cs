using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Application.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFood.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(RegisterUserDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Username,
                FullName = dto.Username
            };

            var result = await _userManager.CreateAsync(user, dto.Password!);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            return "User created successfully";
        }

        public async Task<string> LoginAsync(UserLoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username!);

            if (user == null)
                throw new Exception("Invalid username or password");

            var validPassword = await _userManager.CheckPasswordAsync(user, dto.Password!);

            if (!validPassword)
                throw new Exception("Invalid username or password");

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserId", user.Id),
                new Claim("Username", user.UserName ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    double.Parse(jwtSettings["DurationInMinutes"]!)
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}