using MyFood.Application.Dtos;
using MyFood.Application.DTOs.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}