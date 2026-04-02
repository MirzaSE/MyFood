using System.Collections.Generic;
using System.Threading.Tasks;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface ITokenService
    {
        Task<TokenResult> GenerateTokenAsync(ApplicationUser user, IEnumerable<string> roles);
    }
}
