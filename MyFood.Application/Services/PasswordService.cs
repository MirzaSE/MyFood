using System.Text.RegularExpressions;

namespace MyFood.Application.Services
{
    public class PasswordService : IPasswordService
    {
        private static readonly Regex UppercaseRegex = new("[A-Z]");
        private static readonly Regex LowercaseRegex = new("[a-z]");
        private static readonly Regex DigitRegex = new("\\d");
        private static readonly Regex SpecialCharacterRegex = new("[@$!%*?&]");

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

            if (!SpecialCharacterRegex.IsMatch(password))
            {
                return "Password must contain at least one special character (@$!%*?&).";
            }

            return string.Empty;
        }
    }
}