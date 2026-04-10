using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> FindByIdAsync(string userId);
        Task<ApplicationUser?> FindByUsernameAsync(string username);
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<ApplicationUser?> FindByVerificationTokenAsync(string verificationToken);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<bool> CreateAsync(ApplicationUser user, string password);
        Task<bool> UpdateAsync(ApplicationUser user);
    }
}
