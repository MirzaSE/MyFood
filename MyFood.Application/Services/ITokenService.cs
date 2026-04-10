using MyFood.Application.Dtos;

namespace MyFood.Application.Services;

public interface ITokenService
{
    string GenerateToken(UserDto user);
}
