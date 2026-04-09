using MyFood.Application.Dtos;

namespace MyFood.Application.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterUserDto dto);
        Task<string> LoginAsync(LoginDto dto);
    }
}