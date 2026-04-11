
using MyFood.Application.Dtos;

namespace MyFood.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);

        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);

        Task<AuthResponseDto> VerifyEmailAsync(string email, string token);

        Task<bool> ValidateCredentialsAsync(string username, string password);
    }
}