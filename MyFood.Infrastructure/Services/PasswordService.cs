using System.Text.RegularExpressions;
using BCrypt.Net;
using MyFood.Application.Services;

namespace MyFood.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or whitespace.", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return false;

        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public bool IsPasswordStrong(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        return password.Length >= 8
            && Regex.IsMatch(password, "[A-Z]")
            && Regex.IsMatch(password, "[a-z]")
            && Regex.IsMatch(password, "[0-9]")
            && Regex.IsMatch(password, "[@$!%*?&]");
    }

    public string GetPasswordValidationError(string password)
    {
        if (string.IsNullOrEmpty(password))
            return "Password is required.";

        var errors = new List<string>();

        if (password.Length < 8)
            errors.Add("Password must be at least 8 characters long.");
        if (!Regex.IsMatch(password, "[A-Z]"))
            errors.Add("Password must contain at least one uppercase letter.");
        if (!Regex.IsMatch(password, "[a-z]"))
            errors.Add("Password must contain at least one lowercase letter.");
        if (!Regex.IsMatch(password, "[0-9]"))
            errors.Add("Password must contain at least one digit.");
        if (!Regex.IsMatch(password, "[@$!%*?&]"))
            errors.Add("Password must contain at least one special character (@$!%*?&).");

        return string.Join(" ", errors);
    }
}
