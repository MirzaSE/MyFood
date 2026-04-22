using MyFood.Application.Entities;

namespace MyFood.Application.Services
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user);
    }
}
