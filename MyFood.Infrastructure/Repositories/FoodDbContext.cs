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
        public DbSet<IngredientEntity> Ingredients { get; set; }

        // ADD THIS ENTIRE METHOD BELOW YOUR DBSETS
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Always call the base method first
            base.OnModelCreating(modelBuilder);

            // Enforce the 260 character limit on the Ingredient Name
            modelBuilder.Entity<IngredientEntity>()
                .Property(i => i.Name)
                .HasMaxLength(260);
        }
    }
}
