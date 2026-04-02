using Microsoft.EntityFrameworkCore;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FoodDbContext _context;

        public UserRepository(FoodDbContext context)
        {
            _context = context;
        }

        public Task<ApplicationUser?> GetByUsernameAsync(string username)
        {
            var normalized = username.Trim().ToUpperInvariant();
            return _context.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == normalized);
        }

        public Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            var normalized = email.Trim().ToUpperInvariant();
            return _context.Users.FirstOrDefaultAsync(x => x.NormalizedEmail == normalized);
        }

        public async Task AddAsync(ApplicationUser user)
        {
            await _context.Users.AddAsync(user);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
