using System.Security.Cryptography;
using System.Text;

namespace MyFood.Application.Services
{
    public class VerificationTokenService : IVerificationTokenService
    {
        public string GenerateToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(32);
            var token = Convert.ToBase64String(randomBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');

            return token;
        }

        public DateTime GetTokenExpiryUtc(int minutesValid = 60)
        {
            return DateTime.UtcNow.AddMinutes(minutesValid);
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

            var providedBytes = Encoding.UTF8.GetBytes(token);
            var storedBytes = Encoding.UTF8.GetBytes(storedToken);

            return CryptographicOperations.FixedTimeEquals(providedBytes, storedBytes);
        }
    }
}
