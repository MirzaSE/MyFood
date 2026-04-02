using Microsoft.AspNetCore.Identity;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Identity
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetUserByUsernameAsync(string username)
            => await _userManager.FindByNameAsync(username);

        public async Task<bool> UserExistsAsync(string username)
            => await _userManager.FindByNameAsync(username) != null;

        public async Task<(bool Success, string? Error)> CreateUserAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }
            return (true, null);
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
            => await _userManager.CheckPasswordAsync(user, password);

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
            => await _userManager.GetRolesAsync(user);
    }
}
