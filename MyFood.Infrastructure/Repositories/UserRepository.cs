using Microsoft.AspNetCore.Identity;
using MyFood.Application.Entities;
using MyFood.Application.Services;

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

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> CreateAsync(ApplicationUser user)
        {
            var result = await _userManager.CreateAsync(user);
            return (result.Succeeded, result.Errors.Select(error => error.Description));
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return (result.Succeeded, result.Errors.Select(error => error.Description));
        }
    }
}
