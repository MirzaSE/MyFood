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
            modelBuilder.Entity<IngredientEntity>()
                .Property(x => x.Name)
                .HasMaxLength(260)
                .IsRequired();

            modelBuilder.Entity<IngredientEntity>()
                .Property(x => x.Quantity)
                .IsRequired();

            modelBuilder.Entity<IngredientEntity>()
                .HasOne(x => x.Food)
                .WithMany(x => x.Ingredients)
                .HasForeignKey(x => x.FoodId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
