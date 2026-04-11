using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByUsernameAsync(string username);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task AddAsync(ApplicationUser user);
        Task SaveChangesAsync();
    }
}
