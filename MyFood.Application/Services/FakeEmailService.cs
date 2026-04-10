using Microsoft.Extensions.Logging;

namespace MyFood.Application.Services
{
    public class FakeEmailService : IEmailService
    {
        private readonly ILogger<FakeEmailService> _logger;

        public FakeEmailService(ILogger<FakeEmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendVerificationEmailAsync(string email, string verificationToken)
        {
			// This is Fake email service for test
            _logger.LogInformation("Sending verification email to {Email} with token: {Token}", email, verificationToken);
            
            await Task.CompletedTask;
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            _logger.LogInformation("Sending password reset email to {Email} with token: {Token}", email, resetToken);
            
            await Task.CompletedTask;
        }
    }
}
