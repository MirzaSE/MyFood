using Microsoft.Extensions.Configuration;
using MyFood.Application.Services;

namespace MyFood.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task SendVerificationEmailAsync(string email, string verificationToken)
    {
        // In a real app, this would send via SMTP, SendGrid, etc.
        var frontendUrl = _configuration["AppSettings:FrontendUrl"] ?? "http://localhost:4200";
        var verifyLink = $"{frontendUrl}/verify-email?token={verificationToken}";
        var subject = "Verify your email";
        var body = $"Please verify your email by clicking this link: {verifyLink}";

        Console.WriteLine($"[EmailService] Sending verification email to {email}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Body: {body}");

        return Task.CompletedTask;
    }

    public Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        // Not in current task, but present for interface compatibility.
        var resetLink = $"/reset-password?token={resetToken}";
        Console.WriteLine($"[EmailService] Sending reset password email to {email} ({resetLink})");
        return Task.CompletedTask;
    }
}
