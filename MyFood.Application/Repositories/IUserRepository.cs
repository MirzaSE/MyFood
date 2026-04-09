using MyFood.Application.Dtos;

namespace MyFood.Application.Repositories;

public interface IUserRepository
{
    Task<bool> UserExistsAsync(string username);
    Task<UserDto?> GetByUsernameAsync(string username);
    Task<(bool Success, IEnumerable<string> Errors)> CreateUserAsync(RegisterDto registerDto, string verificationToken, DateTime tokenExpiry);
    Task<bool> CheckPasswordAsync(string username, string password);
    Task<(bool Success, string? Error)> SetEmailVerificationAsync(string token);
    Task<bool> IsEmailVerifiedAsync(string username);
}
