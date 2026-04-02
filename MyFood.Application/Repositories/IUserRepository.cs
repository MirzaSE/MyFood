using MyFood.Domain.Entities;

namespace MyFood.Application.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByUsernameAsync(string username);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<(bool Succeeded, IEnumerable<string> Errors)> CreateAsync(ApplicationUser user);
        Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateAsync(ApplicationUser user);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
    }
}
