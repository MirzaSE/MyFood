using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> FindByUsernameAsync(string username);
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
    }
}
