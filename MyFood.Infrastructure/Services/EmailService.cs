using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MyFood.Application.Services;
using System.Threading.Tasks;

namespace MyFood.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationEmailAsync(string email, string verificationToken, string userId)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var frontendUrl = _configuration["FrontendUrl"] ?? "https://localhost:5001";
            
            var verificationLink = $"{frontendUrl}/api/Auth/verify-email?userId={userId}&token={verificationToken}";
            
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MyFood App", emailSettings["FromEmail"]));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Verify Your Email Address";
            
            message.Body = new TextPart("html")
            {
                Text = $@"
                    <h2>Welcome to MyFood!</h2>
                    <p>Please verify your email address by clicking the link below:</p>
                    <p><a href='{verificationLink}'>Verify Email</a></p>
                    <p>Or copy this link: {verificationLink}</p>
                    <p>This link will expire in 24 hours.</p>
                    <p>If you did not create an account, please ignore this email.</p>
                "
            };
            
            using var client = new SmtpClient();
            await client.ConnectAsync(
                emailSettings["SmtpServer"], 
                int.Parse(emailSettings["SmtpPort"]), 
                SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            // Similar implementation for password reset
            var frontendUrl = _configuration["FrontendUrl"] ?? "https://localhost:5001";
            var resetLink = $"{frontendUrl}/reset-password?token={resetToken}";
            
            var message = new MimeMessage();
            var emailSettings = _configuration.GetSection("EmailSettings");
            message.From.Add(new MailboxAddress("MyFood App", emailSettings["FromEmail"]));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Reset Your Password";
            
            message.Body = new TextPart("html")
            {
                Text = $@"
                    <h2>Password Reset Request</h2>
                    <p>Click the link below to reset your password:</p>
                    <p><a href='{resetLink}'>Reset Password</a></p>
                    <p>This link will expire in 1 hour.</p>
                    <p>If you did not request this, please ignore this email.</p>
                "
            };
            
            using var client = new SmtpClient();
            await client.ConnectAsync(
                emailSettings["SmtpServer"], 
                int.Parse(emailSettings["SmtpPort"]), 
                SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}