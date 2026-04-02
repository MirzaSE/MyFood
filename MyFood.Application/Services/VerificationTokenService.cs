using System;
using System.Security.Cryptography;
using System.Text;

namespace MyFood.Application.Services
{
    public class VerificationTokenService : IVerificationTokenService
    {
        public string GenerateToken()
        {
            Span<byte> buffer = stackalloc byte[32];
            RandomNumberGenerator.Fill(buffer);
            return Convert.ToBase64String(buffer);
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

            var tokenBytes = Encoding.UTF8.GetBytes(token);
            var storedBytes = Encoding.UTF8.GetBytes(storedToken);

            if (tokenBytes.Length != storedBytes.Length)
            {
                return false;
            }

            return CryptographicOperations.FixedTimeEquals(tokenBytes, storedBytes);
        }
    }
}
