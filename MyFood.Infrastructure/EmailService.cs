using Microsoft.Extensions.Logging;
using MyFood.Application.Services;

namespace MyFood.Infrastructure
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendVerificationEmailAsync(string email, string verificationToken)
        {
            _logger.LogInformation("Verification email simulated for {Email}. Token: {Token}", email, verificationToken);
            return Task.CompletedTask;
        }

        public Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            _logger.LogInformation("Password reset email simulated for {Email}. Token: {Token}", email, resetToken);
            return Task.CompletedTask;
        }
    }
}
