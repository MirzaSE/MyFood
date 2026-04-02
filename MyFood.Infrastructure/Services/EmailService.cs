using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyFood.Application.Services;

namespace MyFood.Infrastructure.Services // keep email service outside of application since it deals with SMTP, logging, queues... 
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly string _applicationBaseUrl;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _applicationBaseUrl = configuration["App:BaseUrl"] ?? "http://localhost:8080";
        }

        public Task SendVerificationEmailAsync(string email, string verificationToken)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email is required", nameof(email));
            }

            if (string.IsNullOrWhiteSpace(verificationToken))
            {
                throw new ArgumentException("Verification token is required", nameof(verificationToken));
            }

            var link = BuildVerificationLink(email, verificationToken);
            _logger.LogInformation("Sending verification email to {Email}. Link: {Link}", email, link);
            return Task.CompletedTask;
        }

        public Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            _logger.LogInformation("Sending password reset email to {Email}. Token: {Token}", email, resetToken);
            return Task.CompletedTask;
        }

        private string BuildVerificationLink(string email, string token)
        {
            var escapedEmail = Uri.EscapeDataString(email);
            var escapedToken = Uri.EscapeDataString(token);
            return $"{_applicationBaseUrl.TrimEnd('/')}/api/auth/verify-email?email={escapedEmail}&token={escapedToken}";
        }
    }
}
