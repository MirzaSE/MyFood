using System.Security.Cryptography;

namespace MyFood.Application.Services;

public class VerificationTokenService : IVerificationTokenService
{
    public string GenerateToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
    }

    public bool ValidateToken(string token, string storedToken, DateTime expiryTime)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(storedToken))
        {
            return false;
        }

        if (expiryTime < DateTime.UtcNow)
        {
            return false;
        }

        return string.Equals(token, storedToken, StringComparison.Ordinal);
    }
}
