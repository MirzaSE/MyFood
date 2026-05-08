using Microsoft.EntityFrameworkCore;
using MyFood.Domain.Entities;
using MyFood.Application.Services;

namespace MyFood.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FoodDbContext _context;

        public UserRepository(FoodDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task AddAsync(ApplicationUser user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}