using MyFood.Application.Dtos;
using System.Threading.Tasks;

namespace MyFood.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> VerifyEmailAsync(string userId, string token);
        Task<bool> ValidateCredentialsAsync(string username, string password);
    }
}
