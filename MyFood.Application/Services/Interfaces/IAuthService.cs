using MyFood.Application.Dtos;

namespace MyFood.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}