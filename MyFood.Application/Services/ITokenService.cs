using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);
        DateTime GetExpiration();
    }
}