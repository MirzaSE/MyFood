using MyFood.Application.Services;
using System.Text.RegularExpressions;

namespace MyFood.Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);

        public bool VerifyPassword(string password, string hash)
            => BCrypt.Net.BCrypt.Verify(password, hash);

        public bool IsPasswordStrong(string password)
            => string.IsNullOrEmpty(GetPasswordValidationError(password));

        public string GetPasswordValidationError(string password)
        {
            if (password.Length < 8)
                return "Password must be at least 8 characters.";
            if (!Regex.IsMatch(password, @"[A-Z]"))
                return "Password must contain at least one uppercase letter.";
            if (!Regex.IsMatch(password, @"[a-z]"))
                return "Password must contain at least one lowercase letter.";
            if (!Regex.IsMatch(password, @"[0-9]"))
                return "Password must contain at least one digit.";
            if (!Regex.IsMatch(password, @"[@$!%*?&]"))
                return "Password must contain at least one special character (@$!%*?&).";
            return string.Empty;
        }
    }
}