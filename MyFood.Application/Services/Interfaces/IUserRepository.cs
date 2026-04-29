using MyFood.Domain.Entities;

namespace MyFood.Application.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByUsernameAsync(string username);
        Task<bool> UserExists(string username);
        Task CreateUser(ApplicationUser user);
        Task AddAsync(ApplicationUser user);
    }
}