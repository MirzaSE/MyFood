using MyFood.Application.Services;
using System.Security.Cryptography;

namespace MyFood.Infrastructure.Services
{
    public class VerificationTokenService : IVerificationTokenService
    {
        public string GenerateToken()
        {
            Span<byte> randomBytes = stackalloc byte[32];
            RandomNumberGenerator.Fill(randomBytes);
            return Convert.ToHexString(randomBytes);
        }

        public bool ValidateToken(string token, string storedToken, DateTime expiryTime)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(storedToken))
            {
                return false;
            }

            return string.Equals(token, storedToken, StringComparison.Ordinal)
                && DateTime.UtcNow <= expiryTime;
        }
    }
}