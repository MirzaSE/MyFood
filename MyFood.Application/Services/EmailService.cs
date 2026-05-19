namespace MyFood.Application.Services
{
    public class EmailService : IEmailService
    {
        public Task SendVerificationEmailAsync(string email, string verificationToken)
        {
            Console.WriteLine($"[EmailService] Verification email to {email} with token: {verificationToken}");
            return Task.CompletedTask;
        }

        public Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            Console.WriteLine($"[EmailService] Password reset email to {email} with token: {resetToken}");
            return Task.CompletedTask;
        }
    }
}