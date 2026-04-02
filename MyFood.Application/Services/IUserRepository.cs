using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByUsernameAsync(string username);
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<ApplicationUser?> GetByVerificationTokenAsync(string token);
        Task<bool> UsernameExistsAsync(string username);
        Task<ApplicationUser> CreateAsync(ApplicationUser user, string password);
        Task UpdateAsync(ApplicationUser user);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    }
}
