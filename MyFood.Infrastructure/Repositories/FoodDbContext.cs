using Microsoft.EntityFrameworkCore;
using MyFood.Api.Data.Entities;
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
        public DbSet<IngredientEntity> Ingredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the one-to-many relationship
            modelBuilder.Entity<FoodEntity>()
                .HasMany(f => f.Ingredients)
                .WithOne(i => i.Food)
                .HasForeignKey(i => i.FoodId);
        }
    }
}
