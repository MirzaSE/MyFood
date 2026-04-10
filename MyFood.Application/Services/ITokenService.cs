using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(ApplicationUser user, IList<string> roles);
    }
}
