using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Services;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (model == null)
        {
            ModelState.AddModelError(string.Empty, "Request body cannot be empty.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(BuildValidationResponse());
        }

        var result = await _authService.LoginAsync(model);
        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (model == null)
        {
            ModelState.AddModelError(string.Empty, "Request body cannot be empty.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(BuildValidationResponse());
        }

        var result = await _authService.RegisterAsync(model);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto model)
    {
        if (model == null)
        {
            ModelState.AddModelError(string.Empty, "Request body cannot be empty.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(BuildValidationResponse());
        }

        var result = await _authService.VerifyEmailAsync(model);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    private AuthResponseDto BuildValidationResponse()
    {
        var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage)
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .ToArray();

        return new AuthResponseDto
        {
            Success = false,
            Errors = errors.Length > 0 ? errors : new[] { "One or more validation errors occurred." }
        };
    }
}
