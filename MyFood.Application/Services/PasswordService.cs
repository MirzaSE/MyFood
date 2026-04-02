using System.Text.RegularExpressions;

namespace MyFood.Application.Services
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (password.Length < 8)
                return false;

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return false;

            if (!Regex.IsMatch(password, @"[a-z]"))
                return false;

            if (!Regex.IsMatch(password, @"\d"))
                return false;

            if (!Regex.IsMatch(password, @"[@$!%*?&]"))
                return false;

            return true;
        }

        public string GetPasswordValidationError(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(password))
            {
                return "Password is required.";
            }

            if (password.Length < 8)
            {
                errors.Add("at least 8 characters");
            }

            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                errors.Add("one uppercase letter");
            }

            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                errors.Add("one lowercase letter");
            }

            if (!Regex.IsMatch(password, @"\d"))
            {
                errors.Add("one digit");
            }

            if (!Regex.IsMatch(password, @"[@$!%*?&]"))
            {
                errors.Add("one special character (@$!%*?&)");
            }

            if (errors.Any())
            {
                return $"Password must contain: {string.Join(", ", errors)}.";
            }

            return string.Empty;
        }
    }
}
