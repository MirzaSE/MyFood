using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Services;

namespace MyFood.Api.Controllers.v1;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        if (loginDto == null)
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Login request is required."
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var authResponse = await _authService.LoginAsync(loginDto);
            if (!authResponse.Success)
            {
                return Unauthorized(authResponse);
            }

            return Ok(authResponse);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseDto
            {
                Success = false,
                Message = "An unexpected error occurred during login."
            });
        }
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        if (registerDto == null)
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Registration request is required."
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var authResponse = await _authService.RegisterAsync(registerDto);
            if (!authResponse.Success)
            {
                return BadRequest(authResponse);
            }

            return Ok(authResponse);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseDto
            {
                Success = false,
                Message = "An unexpected error occurred during registration."
            });
        }
    }

    [HttpPost]
    [Route("verify-email")]
    public async Task<ActionResult<AuthResponseDto>> VerifyEmail([FromBody] VerifyEmailDto verifyEmailDto)
    {
        if (verifyEmailDto == null)
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Email verification request is required."
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var response = await _authService.VerifyEmailAsync(verifyEmailDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseDto
            {
                Success = false,
                Message = "An unexpected error occurred during email verification."
            });
        }
    }
}
