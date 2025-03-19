  using Microsoft.EntityFrameworkCore;
using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class FoodDbContext : DbContext
    {
        public FoodDbContext(DbContextOptions<FoodDbContext> options)
            : base(options)
        {
        }
        public DbSet<IngredientEntity> Ingredients { get; set; } = null!;
        public DbSet<FoodEntity> FoodItems { get; set; } = null!;
    }
}
