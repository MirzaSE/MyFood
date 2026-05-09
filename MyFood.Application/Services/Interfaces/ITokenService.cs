using MyFood.Domain.Entities;

namespace MyFood.Application.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user);
    }
}