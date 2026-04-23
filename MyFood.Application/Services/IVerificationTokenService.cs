using System;

namespace MyFood.Application.Services
{
    public interface IVerificationTokenService
    {
        string GenerateToken();
        bool ValidateToken(string token, string storedToken, DateTime expiryTime);
    }
}
