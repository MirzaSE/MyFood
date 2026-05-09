using System.Text.RegularExpressions;

namespace MyFood.Application.Services
{
    public class PasswordService : IPasswordService
    {
        // Hash password using BCrypt
        public string HashPassword(string password)
        {
            // BCrypt automatically handles salting
            // Work factor 12 is good balance between security and performance
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        // Verify password against hash
        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        // Check if password meets strength requirements
        public bool IsPasswordStrong(string password)
        {
            return GetPasswordValidationError(password) == null;
        }

        // Get detailed error message for weak passwords
        public string GetPasswordValidationError(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return "Password is required";
            }

            if (password.Length < 8)
            {
                return "Password must be at least 8 characters long";
            }

            // Check for at least one uppercase letter
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                return "Password must contain at least one uppercase letter";
            }

            // Check for at least one lowercase letter
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                return "Password must contain at least one lowercase letter";
            }

            // Check for at least one digit
            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                return "Password must contain at least one number";
            }

            // Check for at least one special character
            if (!Regex.IsMatch(password, @"[@$!%*?&#]"))
            {
                return "Password must contain at least one special character (@$!%*?&#)";
            }

            // Password is strong
            return null;
        }
    }
}