using Microsoft.AspNetCore.Identity;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IReadOnlyList<string>> GetRolesAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> CreateAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            var errors = result.Errors.Select(e => e.Description);
            return (result.Succeeded, errors);
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> UpdateAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            var errors = result.Errors.Select(e => e.Description);
            return (result.Succeeded, errors);
        }
    }
}
