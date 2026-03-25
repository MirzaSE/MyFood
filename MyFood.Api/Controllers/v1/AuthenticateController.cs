using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticateController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByNameAsync(registerUser.Username!);
            if (existingUser != null)
            {
                return BadRequest(new { Message = "Username already exists." });
            }

            var user = new ApplicationUser
            {
                UserName = registerUser.Username,
                FullName = registerUser.Username
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password!);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto userLogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(userLogin.Username!);
            if (user == null || !await _userManager.CheckPasswordAsync(user, userLogin.Password!))
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var keyValue = _configuration["Jwt:Key"] ?? "C23C21793C3C7B3AB67DCEB614FE8C7B3AB67DCEB614FE8";
            var issuer = _configuration["Jwt:Issuer"] ?? "myfood.domain.com";
            var audience = _configuration["Jwt:Audience"] ?? "myfood.domain.com";
            var durationInMinutes = int.TryParse(_configuration["Jwt:DurationInMinutes"], out var parsedDuration)
                ? parsedDuration
                : 60;

            var claims = new[]
            {
                new Claim("UserId", user.Id),
                new Claim("Username", user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

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
