using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFood.Infrastructure.Services
{
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string? GenerateToken(ApplicationUser user, IEnumerable<string> roles)
        {
            if (user == null)
            {
                return null;
            }

            var secret = _configuration["JWT:Secret"];
            var issuer = _configuration["JWT:ValidIssuer"];
            var audience = _configuration["JWT:ValidAudience"];

            if (string.IsNullOrWhiteSpace(secret) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                return null;
            }

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles ?? Enumerable.Empty<string>())
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.UtcNow.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
