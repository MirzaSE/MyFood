namespace MyFood.Application.Services
{
    public interface IVerificationTokenService
    {
        string GenerateToken();

        DateTime GetTokenExpiryUtc(int minutesValid = 60);

        bool ValidateToken(string token, string storedToken, DateTime expiryTime);
    }
}