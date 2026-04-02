using Microsoft.AspNetCore.Identity;
using MyFood.Application.Models;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AuthUser?> GetByUsernameAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        return user == null ? null : Map(user);
    }

    public async Task<AuthUser?> GetByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user == null ? null : Map(user);
    }

    public async Task<AuthUser> CreateAsync(AuthUser user)
    {
        var identityUser = new ApplicationUser
        {
            UserName = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            PasswordHash = user.PasswordHash,
            EmailConfirmed = user.EmailVerified,
            EmailVerificationToken = user.EmailVerificationToken,
            EmailVerificationTokenExpiryUtc = user.EmailVerificationTokenExpiryUtc,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(identityUser);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed. {errors}");
        }

        return Map(identityUser);
    }

    public async Task<AuthUser> UpdateAsync(AuthUser user)
    {
        var existingUser = await _userManager.FindByIdAsync(user.Id);
        if (existingUser == null)
        {
            throw new InvalidOperationException("User was not found.");
        }

        existingUser.UserName = user.Username;
        existingUser.Email = user.Email;
        existingUser.FullName = user.FullName;
        existingUser.PasswordHash = user.PasswordHash;
        existingUser.EmailConfirmed = user.EmailVerified;
        existingUser.EmailVerificationToken = user.EmailVerificationToken;
        existingUser.EmailVerificationTokenExpiryUtc = user.EmailVerificationTokenExpiryUtc;

        var result = await _userManager.UpdateAsync(existingUser);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User update failed. {errors}");
        }

        return Map(existingUser);
    }

    private static AuthUser Map(ApplicationUser user)
    {
        return new AuthUser
        {
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            PasswordHash = user.PasswordHash ?? string.Empty,
            EmailVerified = user.EmailConfirmed,
            EmailVerificationToken = user.EmailVerificationToken ?? string.Empty,
            EmailVerificationTokenExpiryUtc = user.EmailVerificationTokenExpiryUtc
        };
    }
}
