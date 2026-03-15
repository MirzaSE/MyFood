  using Microsoft.EntityFrameworkCore;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class FoodDbContext : DbContext
    {
        public FoodDbContext(DbContextOptions<FoodDbContext> options)
            : base(options)
        {
        }

        public DbSet<FoodEntity> FoodItems { get; set; } = null!;
        public DbSet<IngredientEntity> IngredientItems { get; set; } = null!;
    }
}
