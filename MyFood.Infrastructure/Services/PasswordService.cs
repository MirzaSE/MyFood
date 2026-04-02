using Microsoft.AspNetCore.Identity;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using System.Text.RegularExpressions;

namespace MyFood.Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        private static readonly Regex StrongPasswordRegex =
            new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$", RegexOptions.Compiled);

        private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();

        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(new ApplicationUser(), password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            var result = _passwordHasher.VerifyHashedPassword(new ApplicationUser(), hash, password);
            return result == PasswordVerificationResult.Success
                || result == PasswordVerificationResult.SuccessRehashNeeded;
        }

        public bool IsPasswordStrong(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && StrongPasswordRegex.IsMatch(password);
        }

        public string GetPasswordValidationError(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return "Password is required.";
            }

            if (password.Length < 8)
            {
                return "Password must be at least 8 characters long.";
            }

            if (!password.Any(char.IsUpper))
            {
                return "Password must contain at least one uppercase letter.";
            }

            if (!password.Any(char.IsLower))
            {
                return "Password must contain at least one lowercase letter.";
            }

            if (!password.Any(char.IsDigit))
            {
                return "Password must contain at least one digit.";
            }

            if (!password.Any(ch => "@$!%*?&".Contains(ch)))
            {
                return "Password must contain at least one special character (@$!%*?&).";
            }

            return string.Empty;
        }
    }
}
