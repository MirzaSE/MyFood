using Microsoft.AspNetCore.Identity;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using System.Text.RegularExpressions;

namespace MyFood.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(new ApplicationUser(), password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            return false;
        }

        var result = _passwordHasher.VerifyHashedPassword(new ApplicationUser(), hash, password);
        return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
    }

    public bool IsPasswordStrong(string password)
    {
        return string.IsNullOrEmpty(GetPasswordValidationError(password));
    }

    public string GetPasswordValidationError(string password)
    {
        if (password.Length < 8)
        {
            return "Password cannot be less than 8 symbols long.";
        }

        if (!Regex.IsMatch(password, "[A-Z]"))
        {
            return "Password must contain at least one uppercase letter.";
        }

        if (!Regex.IsMatch(password, "[a-z]"))
        {
            return "Password must contain at least one lowercase letter.";
        }

        if (!Regex.IsMatch(password, "[0-9]"))
        {
            return "Password must contain at least one number.";
        }

        if (!Regex.IsMatch(password, "[@$!%*?&]"))
        {
            return "Password must contain at least one special character.";
        }

        return string.Empty;
    }
}
