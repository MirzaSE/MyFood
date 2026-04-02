using MyFood.Application.Models;

namespace MyFood.Application.Services;

public interface IUserRepository
{
    Task<AuthUser?> GetByUsernameAsync(string username);
    Task<AuthUser?> GetByEmailAsync(string email);
    Task<AuthUser> CreateAsync(AuthUser user);
    Task<AuthUser> UpdateAsync(AuthUser user);
}
