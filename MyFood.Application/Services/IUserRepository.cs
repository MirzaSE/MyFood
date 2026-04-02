using MyFood.Domain.Entities;

namespace MyFood.Application.Services;

public interface IUserRepository
{
    Task<ApplicationUser?> FindByUsernameAsync(string username);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task<(bool Succeeded, IEnumerable<string> Errors)> CreateAsync(ApplicationUser user, string password);
    Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateAsync(ApplicationUser user);
}
