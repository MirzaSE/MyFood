using System.Security.Cryptography;

namespace MyFood.Application.Services
{
    public class VerificationTokenService : IVerificationTokenService
    {
        public string GenerateToken()
        {
            var tokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }
            return Convert.ToBase64String(tokenBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        public bool ValidateToken(string token, string storedToken, DateTime expiryTime)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(storedToken))
                return false;

            if (DateTime.UtcNow > expiryTime)
                return false;

            return token == storedToken;
        }
    }
}
