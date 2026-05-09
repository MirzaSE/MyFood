using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using System.Threading.Tasks;

namespace MyFood.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Notice: Now depends on IAuthService (Application layer), not UserManager
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Call the service - it handles all business logic
            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
            {
                // Return 400 Bad Request with error message
                return BadRequest(new { error = result.Message });
            }

            // Return 200 OK with the result
            return Ok(result);
        }

        [HttpPost("login")]
public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
{
    // Validate model state
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // Call the service
    var result = await _authService.LoginAsync(loginDto);

    // If login failed, return 401 Unauthorized
    if (!result.Success)
    {
        return Unauthorized(new { error = result.Message });
    }

    // Return success with token
    return Ok(result);
}

[HttpGet("verify-email")]
public async Task<ActionResult<AuthResponseDto>> VerifyEmail([FromQuery] string userId, [FromQuery] string token)
{
    if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
    {
        return BadRequest(new { error = "UserId and token are required" });
    }

    var result = await _authService.VerifyEmailAsync(userId, token);

    if (!result.Success)
    {
        return BadRequest(new { error = result.Message });
    }

    return Ok(result);
}
    }
}
