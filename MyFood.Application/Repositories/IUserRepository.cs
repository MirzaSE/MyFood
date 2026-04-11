using Microsoft.AspNetCore.Identity;
using MyFood.Domain.Entities;

namespace MyFood.Application.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> FindByUsernameAsync(string username);

        Task<ApplicationUser?> FindByEmailAsync(string email);

        Task<IdentityResult> CreateAsync(ApplicationUser user);

        Task<IdentityResult> UpdateAsync(ApplicationUser user);

        Task<IList<string>> GetRolesAsync(ApplicationUser user);
    }
}
