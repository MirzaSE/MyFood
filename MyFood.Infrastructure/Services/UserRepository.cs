using Microsoft.AspNetCore.Identity;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyFood.Infrastructure.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> FindByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<ApplicationUser?> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser?> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<(bool Succeeded, string[] Errors)> CreateAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            
            if (result.Succeeded)
            {
                return (true, new string[0]);
            }
            
            var errors = result.Errors.Select(e => e.Description).ToArray();
            return (false, errors);
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<bool> UpdateAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> SetEmailVerificationTokenAsync(string userId, string token, DateTime expiry)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            
            user.EmailVerificationToken = token;
            user.EmailVerificationTokenExpiry = expiry;
            
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> VerifyEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            
            // Check if token exists and not expired
            if (user.EmailVerificationToken == null || 
                user.EmailVerificationTokenExpiry == null ||
                DateTime.UtcNow > user.EmailVerificationTokenExpiry ||
                user.EmailVerificationToken != token)
            {
                return false;
            }
            
            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;
            
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> IsEmailVerifiedAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            
            return user.IsEmailVerified;
        }
    }
}
