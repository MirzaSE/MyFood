using Microsoft.EntityFrameworkCore;
using MyFood.Application.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MyFood.Infrastructure.Repositories
{
   public class FoodDbContext : IdentityDbContext<ApplicationUser>
{
    public FoodDbContext(DbContextOptions<FoodDbContext> options) 
        : base(options) { }

    // Your other DbSets
    public DbSet<FoodEntity> FoodItems { get; set; }
}
}
