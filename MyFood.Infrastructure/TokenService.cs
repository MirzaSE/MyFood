using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string CreateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username)
        };

        var secret = _config["JWT:Secret"] ?? _config["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(secret) || Encoding.UTF8.GetBytes(secret).Length < 32)
        {
            throw new InvalidOperationException("JWT secret must be at least 32 bytes; set JWT:Secret (or Jwt:Key) to a longer value.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["JWT:ValidIssuer"],
            audience: _config["JWT:ValidAudience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}