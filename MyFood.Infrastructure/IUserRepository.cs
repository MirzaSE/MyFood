using MyFood.Domain.Entities;

namespace MyFood.Application.Services;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task<bool> SaveAsync();
}