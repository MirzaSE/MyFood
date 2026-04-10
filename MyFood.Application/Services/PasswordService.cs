using System.Text.RegularExpressions;

namespace MyFood.Application.Services;

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
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return "Password must be at least 8 characters long.";

        if (!password.Any(char.IsUpper))
            return "Password must contain at least one uppercase letter.";

        if (!password.Any(char.IsLower))
            return "Password must contain at least one lowercase letter.";

        if (!password.Any(char.IsDigit))
            return "Password must contain at least one digit.";

        if (!Regex.IsMatch(password, @"[@$!%*?&]"))
            return "Password must contain at least one special character (@$!%*?&).";

        return string.Empty;
    }
}
