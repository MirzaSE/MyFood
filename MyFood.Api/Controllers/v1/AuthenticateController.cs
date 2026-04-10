using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Services;

namespace MyFood.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticateController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _authService.LoginAsync(model);
            if (!result.Success)
            {
                return Unauthorized();
            }
            
            return Ok(new
            {
                token = result.Token,
                expiration = result.Expiration
            });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
        {
            var result = await _authService.RegisterAsync(model);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
            }
            
            return Ok(result.Message);
        }
    }
}