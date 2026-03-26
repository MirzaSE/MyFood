using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Infrastructure.Services;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticateController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUser)
        {
            var username = registerUser.Username?.Trim();
            var fullName = registerUser.FullName?.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(registerUser.Password))
            {
                return BadRequest(new { Message = "Username, FullName and Password are required." });
            }

            var (success, token, error) = await _authService.RegisterAsync(username, fullName, registerUser.Password);

            if (!success)
            {
                return BadRequest(new { Message = error });
            }

            return Ok(new { Token = token });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var username = loginDto.Username?.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest(new { Message = "Username and Password are required." });
            }

            var (success, token, error) = await _authService.LoginAsync(username, loginDto.Password);

            if (!success)
            {
                return Unauthorized(new { Message = error });
            }

            return Ok(new { Token = token });
        }
    }
}