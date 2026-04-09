public interface IEmailService
{
Task SendVerificationEmailAsync(string email, string verificationToken);
Task SendPasswordResetEmailAsync(string email, string resetToken);
}
// MyFood.Application.Services.IVerificationTokenService.cs
public interface IVerificationTokenService
{
string GenerateToken();
bool ValidateToken(string token, string storedToken, DateTime expiryTime);
}