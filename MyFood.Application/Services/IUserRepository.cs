using MyFood.Domain.Entities;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByUsernameAsync(string username);
    Task AddAsync(ApplicationUser user);
}