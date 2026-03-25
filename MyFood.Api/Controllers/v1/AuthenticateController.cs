using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyFood.Application.Entities;
using MyFood.Application.Dtos;
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
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            if (string.IsNullOrWhiteSpace(registerUserDto.Username) || string.IsNullOrWhiteSpace(registerUserDto.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var existingUser = await _userManager.FindByNameAsync(registerUserDto.Username);
            if (existingUser != null)
            {
                return BadRequest("User already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = registerUserDto.Username,
                FullName = registerUserDto.Username
            };

            var result = await _userManager.CreateAsync(user, registerUserDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null)
            {
                return Unauthorized();
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordValid)
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT key is not configured.");
            var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured.");
            var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT audience is not configured.");
            var durationInMinutes = int.Parse(jwtSettings["DurationInMinutes"] ?? "60");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var expires = DateTime.UtcNow.AddMinutes(durationInMinutes);

            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
