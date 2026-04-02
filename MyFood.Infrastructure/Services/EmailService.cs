using Microsoft.Extensions.Logging;
using MyFood.Application.Services;

namespace MyFood.Infrastructure.Services
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
            _logger.LogInformation("Mock verification email sent to {Email}. Token: {Token}", email, verificationToken);
            return Task.CompletedTask;
        }

        public Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            _logger.LogInformation("Mock password reset email sent to {Email}. Token: {Token}", email, resetToken);
            return Task.CompletedTask;
        }
    }
}
