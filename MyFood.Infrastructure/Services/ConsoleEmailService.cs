using Microsoft.Extensions.Logging;
using MyFood.Application.Services;

namespace MyFood.Infrastructure.Services;

public class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> _logger;

    public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendVerificationEmailAsync(string email, string verificationToken)
    {
        _logger.LogInformation(
            "Verification email prepared for {Email}. Token: {Token}",
            email,
            verificationToken);

        return Task.CompletedTask;
    }

    public Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        _logger.LogInformation(
            "Password reset email prepared for {Email}. Token: {Token}",
            email,
            resetToken);

        return Task.CompletedTask;
    }
}
