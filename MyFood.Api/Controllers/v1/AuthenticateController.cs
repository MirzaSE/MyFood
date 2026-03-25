using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticateController(UserManager<ApplicationUser> userManager, IConfiguration configuration) : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUser)
        {
            var username = registerUser.FullName?.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(registerUser.Password))
            {
                return BadRequest(new { Message = "FullName and Password are required." });
            }

            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser is not null)
            {
                return Conflict(new { Message = "User already exists." });
            }

            var user = new ApplicationUser
            {
                FullName = username,
                UserName = username
            };

            var createResult = await _userManager.CreateAsync(user, registerUser.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(new
                {
                    Message = "Registration failed.",
                    Errors = createResult.Errors.Select(e => e.Description)
                });
            }

            return CreatedAtAction(nameof(Register), new { UserId = user.Id, Username = user.UserName });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var username = loginDto.FullName?.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest(new { Message = "FullName and Password are required." });
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
            {
                return Unauthorized(new { Message = "Invalid credentials." });
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                return Unauthorized(new { Message = "Invalid credentials." });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim("UserId", user.Id),
                new Claim("Username", user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty)
            };

            var jwtKey = _configuration["Jwt:Key"] ?? "C23C21793C3C7B3AB67DCEB614FE8C7B3AB67DCEB614FE8";
            var issuer = _configuration["Jwt:Issuer"] ?? "https://localhost:7124";
            var audience = _configuration["Jwt:Audience"] ?? "https://localhost:7124";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: _configuration["Jwt:DurationInMinutes"] != null ? DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:DurationInMinutes"])) : DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}