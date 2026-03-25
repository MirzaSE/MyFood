using Microsoft.EntityFrameworkCore;
using MyFood.Application.Entities;
using MyFood.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MyFood.Infrastructure.Repositories
{
    public class FoodDbContext : IdentityDbContext<ApplicationUser>
    {
        public FoodDbContext(DbContextOptions<FoodDbContext> options)
            : base(options)
        {
        }

        public DbSet<FoodEntity> FoodItems { get; set; } = null!;
        public DbSet<IngredientEntity> Ingredients { get; set; }

        public DbSet<ApplicationUser> Users { get; set; } = null!;
    }
}
