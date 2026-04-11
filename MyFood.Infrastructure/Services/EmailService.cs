using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyFood.Application.Services;
using System.Net;
using System.Net.Mail;

namespace MyFood.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendVerificationEmailAsync(string email, string verificationLink)
        {
            var subject = "Verify your MyFood account";
            var body = $"Please verify your account by opening this link: {verificationLink}";

            await SendEmailAsync(email, subject, body, verificationLink);
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            var subject = "Reset your MyFood password";
            var body = $"Use this reset token to reset your password: {resetToken}";

            await SendEmailAsync(email, subject, body, resetToken);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body, string fallbackPayload)
        {
            var smtpHost = _configuration["Email:Smtp:Host"];
            var fromEmail = _configuration["Email:Smtp:From"];

            if (string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(fromEmail))
            {
                // In development, log the verification link/token when SMTP is not configured.
                _logger.LogInformation("Email not sent (SMTP not configured). To: {Email}, Payload: {Payload}", toEmail, fallbackPayload);
                return;
            }

            var smtpPort = int.TryParse(_configuration["Email:Smtp:Port"], out var port) ? port : 25;
            var username = _configuration["Email:Smtp:Username"];
            var password = _configuration["Email:Smtp:Password"];
            var enableSsl = bool.TryParse(_configuration["Email:Smtp:EnableSsl"], out var ssl) && ssl;

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = enableSsl
            };

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                client.Credentials = new NetworkCredential(username, password);
            }

            using var message = new MailMessage(fromEmail, toEmail, subject, body);
            await client.SendMailAsync(message);
        }
    }
}
