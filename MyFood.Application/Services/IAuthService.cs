using MyFood.Application.Dtos;

namespace MyFood.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<bool> ValidateCredentialsAsync(string username, string password);
}
