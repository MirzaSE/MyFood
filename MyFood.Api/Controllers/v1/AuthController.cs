using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Services;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (registerDto is null)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Request body is required."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.RegisterAsync(registerDto);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for username {Username}", registerDto.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred while processing the request."
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto is null)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Request body is required."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.LoginAsync(loginDto);

                if (!result.Success)
                {
                    return Unauthorized(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for username {Username}", loginDto.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred while processing the request."
                });
            }
        }

        [HttpGet("/api/auth/verify-email")]
        public async Task<ActionResult<AuthResponseDto>> VerifyEmail([FromQuery] string email, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Email and token are required."
                });
            }

            try
            {
                var result = await _authService.VerifyEmailAsync(email, token);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email verification failed for email {Email}", email);
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred while processing the request."
                });
            }
        }
    }
}