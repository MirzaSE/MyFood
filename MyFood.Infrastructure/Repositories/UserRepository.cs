using Microsoft.AspNetCore.Identity;
using MyFood.Application.Repositories;
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
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            return await _userManager.FindByNameAsync(username.Trim());
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            return await _userManager.FindByEmailAsync(email.Trim());
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> CreateAsync(ApplicationUser user)
        {
            if (user == null)
            {
                return (false, new[] { "User data is required." });
            }

            var result = await _userManager.CreateAsync(user);
            var errors = result.Errors.Select(error => error.Description);

            return (result.Succeeded, errors);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateAsync(ApplicationUser user)
        {
            if (user == null)
            {
                return (false, new[] { "User data is required." });
            }

            var result = await _userManager.UpdateAsync(user);
            var errors = result.Errors.Select(error => error.Description);

            return (result.Succeeded, errors);
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            if (user == null)
            {
                return Array.Empty<string>();
            }

            return await _userManager.GetRolesAsync(user);
        }
    }
}
