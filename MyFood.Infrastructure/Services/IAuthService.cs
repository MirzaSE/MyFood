using MyFood.Domain;

namespace MyFood.Infrastructure.Services;

public interface IAuthService
{
    Task<(bool Success, string? Token, string? Error)> RegisterAsync(string username, string fullName, string password);
    Task<(bool Success, string? Token, string? Error)> LoginAsync(string username, string password);
}