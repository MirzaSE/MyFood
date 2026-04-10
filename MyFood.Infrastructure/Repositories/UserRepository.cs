using Microsoft.AspNetCore.Identity;
using MyFood.Application.Dtos;
using MyFood.Application.Repositories;
using MyFood.Application.Services;
using MyFood.Infrastructure.Repositories.Models;

namespace MyFood.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPasswordService _passwordService;

    public UserRepository(UserManager<ApplicationUser> userManager, IPasswordService passwordService)
    {
        _userManager = userManager;
        _passwordService = passwordService;
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        var user = await _userManager.FindByNameAsync(username);
        return user != null;
    }

    public async Task<UserDto?> GetByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return null;

        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            FullName = user.FullName
        };
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> CreateUserAsync(RegisterDto registerDto, string verificationToken, DateTime tokenExpiry)
    {
        if (registerDto == null || string.IsNullOrWhiteSpace(registerDto.Username) || string.IsNullOrWhiteSpace(registerDto.Password))
            return (false, new[] { "Username and password are required." });

        var existing = await _userManager.FindByNameAsync(registerDto.Username);
        if (existing != null)
            return (false, new[] { "Username already exists." });

        var user = new ApplicationUser
        {
            UserName = registerDto.Username,
            Email = string.IsNullOrWhiteSpace(registerDto.Email) ? registerDto.Username : registerDto.Email,
            FullName = string.IsNullOrWhiteSpace(registerDto.FullName) ? registerDto.Username : registerDto.FullName,
            PasswordHash = _passwordService.HashPassword(registerDto.Password),
            IsEmailVerified = false,
            EmailVerificationToken = verificationToken,
            EmailVerificationTokenExpiresAt = tokenExpiry
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
            return (false, result.Errors.Select(x => x.Description));

        return (true, Array.Empty<string>());
    }

    public async Task<bool> CheckPasswordAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        var user = await _userManager.FindByNameAsync(username);
        if (user == null || string.IsNullOrWhiteSpace(user.PasswordHash))
            return false;

        return _passwordService.VerifyPassword(password, user.PasswordHash);
    }

    public async Task<(bool Success, string? Error)> SetEmailVerificationAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return (false, "Invalid verification token.");

        var user = _userManager.Users.FirstOrDefault(u => u.EmailVerificationToken == token);
        if (user == null)
            return (false, "Verification token not found.");

        if (!user.EmailVerificationTokenExpiresAt.HasValue || user.EmailVerificationTokenExpiresAt.Value < DateTime.UtcNow)
            return (false, "Verification token expired.");

        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiresAt = null;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

        return (true, null);
    }

    public async Task<bool> IsEmailVerifiedAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        var user = await _userManager.FindByNameAsync(username);
        return user?.IsEmailVerified ?? false;
    }
}

