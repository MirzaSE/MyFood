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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IngredientEntity>()
                .Property(i => i.Name)
                .HasMaxLength(260)
                .IsRequired();

            modelBuilder.Entity<IngredientEntity>()
                .Property(i => i.Quantity)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<FoodEntity>()
                .HasMany(f => f.Ingredients)
                .WithOne(i => i.Food)
                .HasForeignKey(i => i.FoodId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
