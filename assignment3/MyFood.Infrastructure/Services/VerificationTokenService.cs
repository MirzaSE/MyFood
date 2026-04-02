using MyFood.Application.Services;

namespace MyFood.Infrastructure.Services;

public class VerificationTokenService : IVerificationTokenService
{
    public string GenerateToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    public bool ValidateToken(string token, string storedToken, DateTime expiryTime)
    {
        return !string.IsNullOrWhiteSpace(token)
            && token == storedToken
            && DateTime.UtcNow <= expiryTime;
    }
}
