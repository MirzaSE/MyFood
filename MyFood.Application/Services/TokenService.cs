using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class TokenService : ITokenService
    {
        public string GenerateToken(ApplicationUser user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("ThisIsMyVerySecretKey123456789_SUPER_LONG_KEY")
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}