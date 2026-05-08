namespace MyFood.Application.Services
{
    public interface IPasswordService
    {
        // Hash a plain text password
        string HashPassword(string password);
        
        // Verify if a password matches a hash
        bool VerifyPassword(string password, string hash);
        
        // Check if password meets strength requirements
        bool IsPasswordStrong(string password);
        
        // Get error message explaining why password is weak
        string GetPasswordValidationError(string password);
    }
}