using System.Security.Cryptography;

namespace MyFood.Application.Services;

public class VerificationTokenService : IVerificationTokenService
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(24);

    public string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    public bool ValidateToken(string token, string storedToken, DateTime expiryTime)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(storedToken))
            return false;

        if (DateTime.UtcNow > expiryTime)
            return false;

        return CryptographicOperations.FixedTimeEquals(
            System.Text.Encoding.UTF8.GetBytes(token),
            System.Text.Encoding.UTF8.GetBytes(storedToken));
    }

    public DateTime GetTokenExpiry() => DateTime.UtcNow.Add(TokenLifetime);
}
