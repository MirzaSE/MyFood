using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using BCrypt.Net;

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
            if (string.IsNullOrWhiteSpace(hash))
                return false;
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
            return GetPasswordValidationError(password) == null;
        }

        public string? GetPasswordValidationError(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return "Password must be at least 8 characters.";
            if (!Regex.IsMatch(password, @"[A-Z]"))
                return "Password must contain at least one uppercase letter.";
            if (!Regex.IsMatch(password, @"[a-z]"))
                return "Password must contain at least one lowercase letter.";
            if (!Regex.IsMatch(password, @"[0-9]"))
                return "Password must contain at least one digit.";
            if (!Regex.IsMatch(password, @"[@$!%*?&]"))
                return "Password must contain at least one special character (@$!%*?&).";
            return null;
        }
    }
}