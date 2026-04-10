namespace MyFood.Application.Services;

public class PasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
        {
            return false;
        }

        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    public bool IsPasswordStrong(string password, out string validationError)
    {
        validationError = string.Empty;

        if (string.IsNullOrWhiteSpace(password))
        {
            validationError = "Password is required.";
            return false;
        }

        if (password.Length < 8)
        {
            validationError = "Password must be at least 8 characters long.";
            return false;
        }

        if (!password.Any(char.IsUpper))
        {
            validationError = "Password must include at least one uppercase letter.";
            return false;
        }

        if (!password.Any(char.IsLower))
        {
            validationError = "Password must include at least one lowercase letter.";
            return false;
        }

        if (!password.Any(char.IsDigit))
        {
            validationError = "Password must include at least one number.";
            return false;
        }

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            validationError = "Password must include at least one special character.";
            return false;
        }

        return true;
    }
}
