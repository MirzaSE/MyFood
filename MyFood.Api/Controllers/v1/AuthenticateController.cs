using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyFood.Application.Dtos;
using MyFood.Infrastructure.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            var existingUser = await _userManager.FindByNameAsync(registerUserDto.Username);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Username already exists." });
            }

            var user = new ApplicationUser
            {
                UserName = registerUserDto.Username,
                FullName = registerUserDto.Username
            };

            var result = await _userManager.CreateAsync(user, registerUserDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    errors = result.Errors.Select(error => error.Description)
                });
            }

            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT key is missing.");
            var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT issuer is missing.");
            var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT audience is missing.");

            var durationInMinutes = int.TryParse(jwtSettings["DurationInMinutes"], out var configuredDuration)
                ? configuredDuration
                : 60;

            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id),
                new Claim("Username", user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(durationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
