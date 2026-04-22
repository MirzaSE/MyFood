using MyFood.Application.Entities;

namespace MyFood.Application.Services
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByUsernameAsync(string username);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<(bool Succeeded, IEnumerable<string> Errors)> CreateAsync(ApplicationUser user);
        Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateAsync(ApplicationUser user);
    }
}
