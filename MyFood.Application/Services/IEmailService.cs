using System.Threading.Tasks;

namespace MyFood.Application.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string email, string verificationToken, string userId);
        Task SendPasswordResetEmailAsync(string email, string resetToken);
    }
}