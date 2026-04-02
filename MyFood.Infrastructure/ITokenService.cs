using MyFood.Domain.Entities;

namespace MyFood.Application.Services;

public interface ITokenService
{
    string CreateToken(User user);
}