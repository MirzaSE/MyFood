using MyFood.Application.Services;
using System.Security.Cryptography;

namespace MyFood.Infrastructure.Services;

public class VerificationTokenService : IVerificationTokenService
{
    public string GenerateToken()
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(tokenBytes);
    }

    public bool ValidateToken(string token, string storedToken, DateTime expiryTime)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(storedToken))
        {
            return false;
        }

        if (!string.Equals(token.Trim(), storedToken.Trim(), StringComparison.Ordinal))
        {
            return false;
        }

        return DateTime.UtcNow <= expiryTime;
    }
}
