using Microsoft.EntityFrameworkCore;
using MyFood.Application.Repositories;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly FoodDbContext _dbContext;

    public UserRepository(FoodDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ApplicationUser?> GetByUsernameAsync(string username)
    {
        return _dbContext.Users.FirstOrDefaultAsync(user => user.UserName == username);
    }

    public Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return _dbContext.Users.FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task AddAsync(ApplicationUser user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public Task UpdateAsync(ApplicationUser user)
    {
        _dbContext.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }
}
