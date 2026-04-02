using MyFood.Domain.Entities;

namespace MyFood.Application.Repositories;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByUsernameAsync(string username);
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task AddAsync(ApplicationUser user);
    Task UpdateAsync(ApplicationUser user);
    Task SaveChangesAsync();
}
