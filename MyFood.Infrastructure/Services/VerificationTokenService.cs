using MyFood.Application.Services;
using System.Security.Cryptography;

namespace MyFood.Infrastructure.Services
{
    public class VerificationTokenService : IVerificationTokenService
    {
        public string GenerateToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public bool ValidateToken(string token, string storedToken, DateTime expiryTime)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(storedToken))
            {
                return false;
            }

            if (DateTime.UtcNow > expiryTime)
            {
                return false;
            }

            return string.Equals(token.Trim(), storedToken.Trim(), StringComparison.Ordinal);
        }
    }
}
