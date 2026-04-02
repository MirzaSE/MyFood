namespace MyFood.Application.Services
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
        bool IsPasswordStrong(string password);
        string? GetPasswordValidationError(string password);
    }
}