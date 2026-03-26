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
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthenticateController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            var userExists = await _userManager.FindByNameAsync(registerUserDto.Username);
            if (userExists != null)
            {
                return BadRequest(new { message = "User already exists!" });
            }

            ApplicationUser user = new ApplicationUser
            {
                UserName = registerUserDto.Username,
                FullName = registerUserDto.Username,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerUserDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "User creation failed!", errors = result.Errors });
            }

            return Ok(new { message = "User created successfully!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials!" });
            }

            var result = await _signInManager.PasswordSignInAsync(
                loginDto.Username, 
                loginDto.Password, 
                false, 
                false);

            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid credentials!" });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
                new Claim("userId", user.Id),
                new Claim("username", user.UserName!)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(jwtSettings["DurationInMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
