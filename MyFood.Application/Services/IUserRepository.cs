using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByUsernameAsync(string username);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<IReadOnlyList<string>> GetRolesAsync(ApplicationUser user);
        Task<(bool Success, IEnumerable<string> Errors)> CreateAsync(ApplicationUser user, string password);
        Task<(bool Success, IEnumerable<string> Errors)> UpdateAsync(ApplicationUser user);
    }
}
