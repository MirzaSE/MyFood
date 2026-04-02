using Microsoft.AspNetCore.Identity;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services;

public class BcryptPasswordHasher : IPasswordHasher<ApplicationUser>
{
    public string HashPassword(ApplicationUser user, string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public PasswordVerificationResult VerifyHashedPassword(
        ApplicationUser user, string hashedPassword, string providedPassword)
    {
        bool valid = BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        return valid ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
    }
}
