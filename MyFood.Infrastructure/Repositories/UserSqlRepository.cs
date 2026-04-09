using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Infrastructure.Repositories
{
    public class UserSqlRepository : IUserRepository
    {
        private readonly FoodDbContext _context;
        public UserSqlRepository(FoodDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;
            return new UserDto {
                Id = user.Id ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty
            };
        }

        public async Task<UserDto?> GetByIdAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;
            return new UserDto {
                Id = user.Id ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty
            };
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<(UserDto User, string PasswordHash)?> FindForLoginAsync(string emailOrUsername)
        {
            if (string.IsNullOrWhiteSpace(emailOrUsername))
                return null;

            var key = emailOrUsername.Trim();
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == key ||
                u.UserName == key ||
                u.NormalizedUserName == key.ToUpperInvariant() ||
                u.FullName == key);

            if (user == null)
                return null;

            var dto = new UserDto
            {
                Id = user.Id ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty
            };

            return (dto, user.PasswordHash ?? string.Empty);
        }

        public async Task<UserDto> CreateAsync(UserDto userDto, string passwordHash)
        {
            var user = new ApplicationUser
            {
                UserName = userDto.FullName ?? string.Empty,
                Email = userDto.Email ?? string.Empty,
                PasswordHash = passwordHash
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            userDto.Id = user.Id ?? string.Empty;
            userDto.FullName = user.FullName ?? string.Empty;
            userDto.Email = user.Email ?? string.Empty;
            return userDto;
        }
    }
}