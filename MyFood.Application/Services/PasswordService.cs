using System.Text.RegularExpressions;

namespace MyFood.Application.Services
{
    public class PasswordService : IPasswordService
    {
        public bool IsPasswordStrong(string password)
        {
            return string.IsNullOrEmpty(GetPasswordValidationError(password));
        }

        public string GetPasswordValidationError(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "Password cannot be empty.";
                
            if (password.Length < 8)
                return "Password must be at least 8 characters long.";
                
            if (!Regex.IsMatch(password, @"[A-Z]"))
                return "Password must contain at least one uppercase letter.";
                
            if (!Regex.IsMatch(password, @"[a-z]"))
                return "Password must contain at least one lowercase letter.";
                
            if (!Regex.IsMatch(password, @"[0-9]"))
                return "Password must contain at least one digit.";
                
            if (!Regex.IsMatch(password, @"[@$!%*?&]"))
                return "Password must contain at least one special character (@$!%*?&).";

            return string.Empty; // Returning empty means there are no errors (it is valid)
        }
    }
}