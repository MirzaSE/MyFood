using MyFood.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace MyFood.Application.Services
{
    public interface IUserRepository
    {
        // Find user by username
        Task<ApplicationUser?> FindByUsernameAsync(string username);
        
        // Find user by email
        Task<ApplicationUser?> FindByEmailAsync(string email);
        
        // Find user by ID
        Task<ApplicationUser?> FindByIdAsync(string userId);
        
        // Create a new user
        Task<(bool Succeeded, string[] Errors)> CreateAsync(ApplicationUser user, string password);
        
        // Validate user's password
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        
        // Update user
        Task<bool> UpdateAsync(ApplicationUser user);
        
        // Delete user
        Task<bool> DeleteAsync(string userId);
        
        // Email verification methods
        Task<bool> SetEmailVerificationTokenAsync(string userId, string token, DateTime expiry);
        Task<bool> VerifyEmailAsync(string userId, string token);
        Task<bool> IsEmailVerifiedAsync(string userId);
    }
}
