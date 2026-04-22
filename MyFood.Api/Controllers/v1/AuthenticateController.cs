using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Services;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Login payload is required."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);
            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPost("verify-email")]
        public async Task<ActionResult<AuthResponseDto>> VerifyEmail([FromBody] VerifyEmailDto verifyEmailDto)
        {
            if (verifyEmailDto == null)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Verification payload is required."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.VerifyEmailAsync(verifyEmailDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
