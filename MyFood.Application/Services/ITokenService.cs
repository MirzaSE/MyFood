using MyFood.Domain.Entities;  // ← Add this using
using System.Threading.Tasks;

namespace MyFood.Application.Services
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(ApplicationUser user);
    }
}
