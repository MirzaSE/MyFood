namespace MyFood.Application.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string email, string verificationLink);

        Task SendPasswordResetEmailAsync(string email, string resetToken);
    }
}