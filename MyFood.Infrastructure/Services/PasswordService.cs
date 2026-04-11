using System.Text.RegularExpressions;
using MyFood.Application.Services;

namespace MyFood.Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        private static readonly Regex StrongPasswordRegex =
            new("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&]).{8,}$", RegexOptions.Compiled);

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            return StrongPasswordRegex.IsMatch(password);
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

            var hasSpecial = password.Any(x => "@$!%*?&".Contains(x));
            if (!hasSpecial)
            {
                return "Password must contain at least one special character (@$!%*?&).";
            }

            return string.Empty;
        }
    }
}
