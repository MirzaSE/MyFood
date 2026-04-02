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

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return _userManager.CreateAsync(user, password);
        }

        public Task<ApplicationUser?> FindByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return Task.FromResult<ApplicationUser?>(null);
            }

            return _userManager.FindByNameAsync(username);
        }

        public Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            if (user == null || string.IsNullOrWhiteSpace(password))
            {
                return Task.FromResult(false);
            }

            return _userManager.CheckPasswordAsync(user, password);
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
    }
}
