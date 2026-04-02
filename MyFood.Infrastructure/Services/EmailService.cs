using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyFood.Application.Services;
using System.Net;

namespace MyFood.Infrastructure.Services;

public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<EmailService> _logger = logger;

    public Task SendVerificationEmailAsync(string email, string verificationToken)
    {
        var baseUrl = _configuration["App:BaseUrl"] ?? "https://localhost:7124";
        var encodedToken = WebUtility.UrlEncode(verificationToken);
        var verificationLink = $"{baseUrl}/api/auth/verify-email?emailOrUsername={WebUtility.UrlEncode(email)}&token={encodedToken}";

        _logger.LogInformation("Verification email prepared for {Email}. Verification link: {VerificationLink}", email, verificationLink);

        return Task.CompletedTask;
    }

    public Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        _logger.LogInformation("Password reset email prepared for {Email}. Reset token: {ResetToken}", email, resetToken);

        return Task.CompletedTask;
    }
}
