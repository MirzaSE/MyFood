using MyFood.Domain.Entities;

namespace MyFood.Application.Services;

public interface ITokenService
{
    Task<(string Token, DateTime Expiration)> GenerateTokenAsync(ApplicationUser user, IList<string> roles);
}
