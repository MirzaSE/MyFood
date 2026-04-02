using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyFood.Application.Repositories;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private const string TokenProvider = "MyFood";
        private const string EmailVerificationTokenName = "EmailVerification";

        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<ApplicationUser?> GetByEmailVerificationTokenAsync(string token)
        {
            return await _userManager.Users.FirstOrDefaultAsync(user => user.EmailVerificationToken == token);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> CreateAsync(ApplicationUser user)
        {
            var result = await _userManager.CreateAsync(user);
            var errors = result.Errors.Select(e => e.Description);
            return (result.Succeeded, errors);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            var errors = result.Errors.Select(e => e.Description);
            return (result.Succeeded, errors);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> SaveEmailVerificationTokenAsync(ApplicationUser user, string token)
        {
            var result = await _userManager.SetAuthenticationTokenAsync(user, TokenProvider, EmailVerificationTokenName, token);
            return result.Succeeded
                ? (true, Enumerable.Empty<string>())
                : (false, result.Errors.Select(e => e.Description));
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> RemoveEmailVerificationTokenAsync(ApplicationUser user)
        {
            var result = await _userManager.RemoveAuthenticationTokenAsync(user, TokenProvider, EmailVerificationTokenName);
            return result.Succeeded
                ? (true, Enumerable.Empty<string>())
                : (false, result.Errors.Select(e => e.Description));
        }
    }
}
