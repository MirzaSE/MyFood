using MyFood.Application.Services;

namespace MyFood.Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);

        public bool VerifyPassword(string password, string hash)
            => BCrypt.Net.BCrypt.Verify(password, hash);

        public bool IsPasswordStrong(string password)
            => GetPasswordValidationError(password) == null;

        public string? GetPasswordValidationError(string password)
        {
            if (password.Length < 8)
                return "Password must be at least 8 characters long.";

            if (!password.Any(char.IsUpper))
                return "Password must contain at least one uppercase letter.";

            if (!password.Any(char.IsLower))
                return "Password must contain at least one lowercase letter.";

            if (!password.Any(char.IsDigit))
                return "Password must contain at least one digit.";

            if (!password.Any(c => "@$!%*?&".Contains(c)))
                return "Password must contain at least one special character (@$!%*?&).";

            return null;
        }
    }
}
