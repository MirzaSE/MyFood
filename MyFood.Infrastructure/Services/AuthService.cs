using Microsoft.AspNetCore.Identity;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IPasswordService passwordService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _passwordService = passwordService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto)
        {
            // Validate password strength
            var passwordError = _passwordService.GetPasswordValidationError(registerDto.Password);
            if (!string.IsNullOrEmpty(passwordError))
                return new AuthResponseDto { Success = false, Message = passwordError };

            // Check if user already exists
            var existingUser = await _userManager.FindByNameAsync(registerDto.Username);
            if (existingUser != null)
                return new AuthResponseDto { Success = false, Message = "Username already exists." };

            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponseDto { Success = false, Message = errors };
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully."
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return new AuthResponseDto { Success = false, Message = "Invalid username or password." };

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                Expiration = _tokenService.GetExpiration()
            };
        }
    }
}