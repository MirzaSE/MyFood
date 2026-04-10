using MyFood.Application.Services;
using System.Text.RegularExpressions;

namespace MyFood.Infrastructure
{
    public class PasswordService : IPasswordService
    {
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

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return false;
            }
        }

        public bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                return false;
            }

            var hasUpper = Regex.IsMatch(password, "[A-Z]");
            var hasLower = Regex.IsMatch(password, "[a-z]");
            var hasDigit = Regex.IsMatch(password, "[0-9]");
            var hasSpecial = Regex.IsMatch(password, "[@$!%*?&]");

            return hasUpper && hasLower && hasDigit && hasSpecial;
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
                return "Password must contain at least one digit.";
            }

            if (!Regex.IsMatch(password, "[@$!%*?&]"))
            {
                return "Password must contain at least one special character (@$!%*?&).";
            }

            return string.Empty;
        }
    }
}
