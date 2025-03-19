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

        public DbSet<FoodEntity> FoodItems { get; set; } = null!;
        public DbSet<IngredientEntity> Ingredients { get; set; } = null!; 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed data
        modelBuilder.Entity<FoodEntity>().HasData(
            new FoodEntity { Id = 1, Name = "Pizza", Type = "Main", Calories = 800, Created = DateTime.UtcNow },
            new FoodEntity { Id = 2, Name = "Salad", Type = "Starter", Calories = 200, Created = DateTime.UtcNow }
        );

        modelBuilder.Entity<IngredientEntity>().HasData(
            new IngredientEntity { Id = 1, Name = "Tomato", FoodEntityId = 1 },
            new IngredientEntity { Id = 2, Name = "Cheese", FoodEntityId = 1 },
            new IngredientEntity { Id = 3, Name = "Lettuce", FoodEntityId = 2 }
        );
    }
    }
}