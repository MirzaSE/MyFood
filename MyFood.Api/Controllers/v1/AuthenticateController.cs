using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyFood.Application.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    public class AuthenticateController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto userLogin)
        {
            // Validate user credentials (this is just an example, use a proper validation method)
            if (userLogin.Username == "test" && userLogin.Password == "password")
            {
                var token = GenerateJwtToken(userLogin.Username);
                return Ok(new { Token = token });
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserLoginDto userLogin)
        {
            //validate user credentials
            if (userLogin.Username == "test" && userLogin.Password == "password")
            {
                var token = GenerateJwtToken(userLogin.Username);
                return Ok(new { Token = token });
            }
            return Unauthorized();
        }

        private string GenerateJwtToken(string username)
        {
            var claims = new[]
            { 
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                // Add more claims if needed
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("C23C21793C3C7B3AB67DCEB614FE8C7B3AB67DCEB614FE8"));  // TODO Get from appsettings
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // TODO Get from appsettings
            var token = new JwtSecurityToken(
                issuer: "myfood.domain.com",
                audience: "myfood.domain.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
