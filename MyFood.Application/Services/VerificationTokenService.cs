using System;
using System.Security.Cryptography;

namespace MyFood.Application.Services
{
    public class VerificationTokenService : IVerificationTokenService
    {
        public string GenerateToken()
        {
            // Generate a secure random token
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        public bool ValidateToken(string token, string storedToken, DateTime expiryTime)
        {
            // Check if token is null or expired
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(storedToken))
                return false;
                
            if (DateTime.UtcNow > expiryTime)
                return false;
                
            // Compare tokens
            return token.Equals(storedToken, StringComparison.Ordinal);
        }
    }
}