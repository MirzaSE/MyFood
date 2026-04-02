using MyFood.Application.Services;
using System.Text.RegularExpressions;

namespace MyFood.Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        private static readonly Regex UppercaseRegex = new("[A-Z]", RegexOptions.Compiled);
        private static readonly Regex LowercaseRegex = new("[a-z]", RegexOptions.Compiled);
        private static readonly Regex DigitRegex = new("[0-9]", RegexOptions.Compiled);
        private static readonly Regex SpecialCharRegex = new("[@$!%*?&]", RegexOptions.Compiled);

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public bool IsPasswordStrong(string password)
        {
            return string.IsNullOrEmpty(GetPasswordValidationError(password));
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

            if (!UppercaseRegex.IsMatch(password))
            {
                return "Password must contain at least one uppercase letter.";
            }

            if (!LowercaseRegex.IsMatch(password))
            {
                return "Password must contain at least one lowercase letter.";
            }

            if (!DigitRegex.IsMatch(password))
            {
                return "Password must contain at least one digit.";
            }

            if (!SpecialCharRegex.IsMatch(password))
            {
                return "Password must contain at least one special character (@$!%*?&).";
            }

            return string.Empty;
        }
    }
}