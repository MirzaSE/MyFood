using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyFood.Application.Services;

namespace MyFood.Infrastructure
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

        public async Task SendVerificationEmailAsync(string email, string token)
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var senderEmail = _configuration["Email:SenderEmail"];
            var username = _configuration["Email:Username"];
            var password = _configuration["Email:Password"];
            var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "http://localhost:8080";
            var verificationUrl = $"{baseUrl}/api/auth/verify-email?token={Uri.EscapeDataString(token)}";

            if (string.IsNullOrWhiteSpace(smtpHost)
                || string.IsNullOrWhiteSpace(senderEmail)
                || string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning(
                    "Email settings are incomplete. Verification email for {Email} was not sent. Use token: {Token}. Verification URL: {VerificationUrl}",
                    email,
                    token,
                    verificationUrl);
                return;
            }

            var senderName = _configuration["Email:SenderName"] ?? "MyFood API";
            var smtpPort = int.TryParse(_configuration["Email:SmtpPort"], out var parsedPort) ? parsedPort : 587;
            var enableSsl = bool.TryParse(_configuration["Email:EnableSsl"], out var parsedSsl) && parsedSsl;

            using var message = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = "Verify your MyFood account",
                Body = $"Please verify your account by visiting: {verificationUrl}",
                IsBodyHtml = false
            };

            message.To.Add(email);

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            };

            await client.SendMailAsync(message);
        }
    }
}