using MyFood.Application.Services;
using System.Security.Cryptography;

namespace MyFood.Infrastructure
{
    public class VerificationTokenService : IVerificationTokenService
    {
        public string GenerateToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        }

        public bool ValidateToken(string token, string storedToken, DateTime expiryTime)
        {
            return !string.IsNullOrWhiteSpace(token)
                   && token == storedToken
                   && DateTime.UtcNow <= expiryTime;
        }
    }
}
