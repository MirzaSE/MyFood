namespace MyFood.Application.Services
{
    public interface IPasswordService
    {
        bool IsPasswordStrong(string password);
        string GetPasswordValidationError(string password);
    }
}