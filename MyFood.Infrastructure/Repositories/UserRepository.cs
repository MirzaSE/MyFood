using Microsoft.AspNetCore.Identity;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public Task<ApplicationUser?> GetByUsernameAsync(string username)
        {
            return _userManager.FindByNameAsync(username);
        }

        public Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> UserExistsAsync(string username, string? email)
        {
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser is not null)
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(email)
                && await _userManager.FindByEmailAsync(email) is not null;
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> CreateAsync(ApplicationUser user)
        {
            var result = await _userManager.CreateAsync(user);
            return (result.Succeeded, result.Errors.Select(x => x.Description));
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            return _userManager.GetRolesAsync(user);
        }

        public async Task EnsureRoleExistsAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        public async Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}
