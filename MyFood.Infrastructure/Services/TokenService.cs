using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyFood.Domain.Entities;
using MyFood.Application.Services.Interfaces;

namespace MyFood.Application.Services
{
    public class TokenService : ITokenService
    {
        private const string SecretKey = "ThisIsMyVerySecretKey123456789_SUPER_LONG_KEY";

        public string GenerateToken(ApplicationUser user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName))
                throw new ArgumentException("User or UserName cannot be null");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}