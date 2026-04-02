using MyFood.Domain.Entities;

namespace MyFood.Application.Services;

public interface IUserRepository
{
    Task<ApplicationUser?> FindByUsernameAsync(string username);
    Task<bool> CreateAsync(ApplicationUser user, string password);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
}
