using MyFood.Domain.Entities;

namespace MyFood.Application.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByUsernameAsync(string username);
        Task<ApplicationUser?> GetByEmailVerificationTokenAsync(string token);
        Task<(bool Succeeded, IEnumerable<string> Errors)> CreateAsync(ApplicationUser user);
        Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateAsync(ApplicationUser user);
        Task<(bool Succeeded, IEnumerable<string> Errors)> SaveEmailVerificationTokenAsync(ApplicationUser user, string token);
        Task<(bool Succeeded, IEnumerable<string> Errors)> RemoveEmailVerificationTokenAsync(ApplicationUser user);
    }
}
