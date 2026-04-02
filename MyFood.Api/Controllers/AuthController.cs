using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Services;

namespace MyFood.Api.Controllers;
 
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
 
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
 
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        if (registerDto == null || string.IsNullOrWhiteSpace(registerDto.Username) || string.IsNullOrWhiteSpace(registerDto.Password))
            return BadRequest(new AuthResponseDto { Success = false, Message = "Username and password are required." });
 
var response = await _authService.RegisterAsync(registerDto);

if (!(response.Success ?? false))
{
    if (response.Message?.Contains("already exists", StringComparison.OrdinalIgnoreCase) == true)
        return Conflict(response);

    return BadRequest(response);
}
 
        return Ok(response);
    }
 
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            return BadRequest(new AuthResponseDto { Success = false, Message = "Username and password are required." });
 
        var response = await _authService.LoginAsync(loginDto);
        if (!(response.Success ?? false))
            return Unauthorized(response);
 
        return Ok(response);
    }
}
 