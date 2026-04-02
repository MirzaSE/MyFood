using MyFood.Application.Models;

namespace MyFood.Application.Services;

public interface ITokenService
{
    string GenerateToken(AuthUser user);
}
