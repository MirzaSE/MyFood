using Microsoft.AspNetCore.Identity;
using MyFood.Application.Entities;

namespace MyFood.Application.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByUsernameAsync(string username);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<IdentityResult> CreateAsync(ApplicationUser user);
        Task<IdentityResult> UpdateAsync(ApplicationUser user);
    }
}