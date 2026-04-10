using MyFood.Domain.Entities;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user);
}