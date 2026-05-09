namespace MyFood.Application.Services;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
    bool IsPasswordStrong(string password, out string validationError);
}
