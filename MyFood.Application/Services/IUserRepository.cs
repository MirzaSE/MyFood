using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByUsernameAsync(string username);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<bool> UserExistsAsync(string username, string? email);
        Task<(bool Succeeded, IEnumerable<string> Errors)> CreateAsync(ApplicationUser user);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task EnsureRoleExistsAsync(string roleName);
        Task AddToRoleAsync(ApplicationUser user, string roleName);
    }
}
