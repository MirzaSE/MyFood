namespace MyFood.Application.Services
{
    public interface IVerificationTokenService
    {
        string GenerateToken();
        bool IsTokenValid(DateTime? expiry);
    }
}