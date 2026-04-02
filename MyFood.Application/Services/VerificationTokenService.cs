using System.Security.Cryptography;

namespace MyFood.Application.Services
{
    public class VerificationTokenService : IVerificationTokenService
    {
        public string GenerateToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }

        public bool IsTokenValid(DateTime? expiry)
        {
            return expiry.HasValue && expiry.Value > DateTime.UtcNow;
        }
    }
}