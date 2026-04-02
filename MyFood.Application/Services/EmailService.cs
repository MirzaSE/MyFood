using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace MyFood.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendVerificationEmailAsync(string email, string verificationToken)
    {
        var baseUrl = _configuration["App:BaseUrl"] ?? "http://localhost:8080";
        var link = $"{baseUrl}/api/auth/verify-email?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(verificationToken)}";

        _logger.LogInformation(
            "Email verification link for {Email}: {Link}", email, link);

        await SendEmailAsync(
            to: email,
            subject: "Verify your MyFood account",
            body: $"Hello,\n\nPlease verify your email by clicking the link below:\n\n{link}\n\nThis link expires in 24 hours.\n\nIf you did not register, ignore this email.");
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        _logger.LogInformation(
            "Password reset token for {Email}: {Token}", email, resetToken);

        await SendEmailAsync(
            to: email,
            subject: "Reset your MyFood password",
            body: $"Hello,\n\nYour password reset token is:\n\n{resetToken}\n\nThis token expires in 1 hour.\n\nIf you did not request a reset, ignore this email.");
    }

    private async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpHost = _configuration["Email:SmtpHost"];
        if (string.IsNullOrEmpty(smtpHost))
        {
            _logger.LogWarning(
                "SMTP is not configured. Email to {To} was NOT sent.\nSubject: {Subject}\nBody:\n{Body}",
                to, subject, body);
            return;
        }

        var port = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        var username = _configuration["Email:Username"];
        var password = _configuration["Email:Password"];
        var from = _configuration["Email:From"] ?? username;

        using var client = new SmtpClient(smtpHost, port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(username, password)
        };

        using var message = new MailMessage(from!, to, subject, body);
        await client.SendMailAsync(message);

        _logger.LogInformation("Email sent to {To} — Subject: {Subject}", to, subject);
    }
}
