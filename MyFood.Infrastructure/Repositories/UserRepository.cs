using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordService _passwordService;

        public UserRepository(UserManager<ApplicationUser> userManager, IPasswordService passwordService)
        {
            _userManager = userManager;
            _passwordService = passwordService;
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(password);
            user.PasswordHash = _passwordService.HashPassword(password);
            return _userManager.CreateAsync(user);
        }

        public Task<ApplicationUser?> FindByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return Task.FromResult<ApplicationUser?>(null);
            }

            return _userManager.FindByNameAsync(username);
        }

        public Task<ApplicationUser?> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Task.FromResult<ApplicationUser?>(null);
            }

            return _userManager.FindByEmailAsync(email);
        }

        public Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            if (user == null || string.IsNullOrWhiteSpace(password))
            {
                return Task.FromResult(false);
            }

            var hashedPassword = user.PasswordHash;
            if (string.IsNullOrWhiteSpace(hashedPassword))
            {
                return Task.FromResult(false);
            }

            var isValid = _passwordService.VerifyPassword(password, hashedPassword);
            return Task.FromResult(isValid);
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            if (user == null)
            {
                IList<string> emptyRoles = Array.Empty<string>();
                return Task.FromResult(emptyRoles);
            }

            return _userManager.GetRolesAsync(user);
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return _userManager.UpdateAsync(user);
        }
    }
}
