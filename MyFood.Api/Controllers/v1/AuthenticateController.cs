using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Services;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/auth")]
    [Route("api/authenticate")]
    [Route("api/v{version:apiVersion}/auth")]
    [Route("api/v{version:apiVersion}/authenticate")]
    public class AuthenticateController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUser)
        {
            var response = await _authService.RegisterAsync(registerUser);
            return StatusCode(response.StatusCode, response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            return StatusCode(response.StatusCode, response);
        }

        [AllowAnonymous]
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string emailOrUsername, [FromQuery] string token)
        {
            var response = await _authService.VerifyEmailAsync(emailOrUsername, token);
            return StatusCode(response.StatusCode, response);
        }
    }

}