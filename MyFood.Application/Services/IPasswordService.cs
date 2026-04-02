using MyFood.Application.Services.Passwords;

namespace MyFood.Application.Services
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
        PasswordValidationResult ValidateStrength(string password);
    }
}
