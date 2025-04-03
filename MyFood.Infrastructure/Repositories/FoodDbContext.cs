using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFood.Application.Entities;
using YourProjectNamespace.Models;

namespace MyFood.Infrastructure.Repositories
{
    public class FoodDbContext(DbContextOptions<FoodDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<FoodEntity> FoodItems { get; set; } = null!;
    }


}
