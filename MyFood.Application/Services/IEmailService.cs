namespace MyFood.Application.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string email, string token);
    }
}