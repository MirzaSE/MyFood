using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyFood.Application.Services;
using System.Net;

namespace MyFood.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly string _verificationEndpoint;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _verificationEndpoint = configuration["App:VerificationEndpoint"] ?? "https://localhost:5001/api/auth/verify-email";
        }

        public Task SendVerificationEmailAsync(string email, string verificationToken)
        {
            var verificationLink =
                $"{_verificationEndpoint}?email={WebUtility.UrlEncode(email)}&token={WebUtility.UrlEncode(verificationToken)}";

            _logger.LogInformation(
                "Verification email prepared for {Email}. Verification link: {VerificationLink}",
                email,
                verificationLink);

            return Task.CompletedTask;
        }

        public Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            _logger.LogInformation(
                "Password reset email prepared for {Email}. Reset token: {ResetToken}",
                email,
                resetToken);

            return Task.CompletedTask;
        }
    }
}