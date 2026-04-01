using Microsoft.AspNetCore.Identity;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Username,
                FullName = dto.Username
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            return new AuthResponseDto
            {
                Success = result.Succeeded,
                Message = result.Succeeded ? "User created" : "Failed"
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);

            if (user == null)
                return new AuthResponseDto { Success = false, Message = "Invalid login" };

            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);

            return new AuthResponseDto
            {
                Success = valid,
                Message = valid ? "Login OK" : "Invalid login"
            };
        }
    }
}