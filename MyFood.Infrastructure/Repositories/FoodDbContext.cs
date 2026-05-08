using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class FoodDbContext : IdentityDbContext<ApplicationUser>
    {
        public FoodDbContext(DbContextOptions<FoodDbContext> options)
            : base(options)
        {
        }

        public DbSet<FoodEntity> FoodItems { get; set; } = null!;
        public DbSet<IngredientEntity> Ingredients { get; set; } = null!; 
        public DbSet<FoodIngredientEntity> FoodIngredients { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FoodIngredientEntity>()
                .HasKey(x => new { x.FoodEntityId, x.IngredientEntityId });

            modelBuilder.Entity<FoodIngredientEntity>()
                .HasOne(x => x.FoodEntity)
                .WithMany(f => f.FoodIngredients)
                .HasForeignKey(x => x.FoodEntityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FoodIngredientEntity>()
                .HasOne(x => x.IngredientEntity)
                .WithMany(i => i.FoodIngredients)
                .HasForeignKey(x => x.IngredientEntityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FoodIngredientEntity>()
                .Property(x => x.Quantity)
                .HasPrecision(18, 2);

            modelBuilder.Entity<IngredientEntity>()
                .Property(x => x.CaloriesPerUnit)
                .HasPrecision(18, 2);

            modelBuilder.Entity<IngredientEntity>()
                .Property(x => x.Protein)
                .HasPrecision(18, 2);

            modelBuilder.Entity<IngredientEntity>()
                .Property(x => x.Carbs)
                .HasPrecision(18, 2);

            modelBuilder.Entity<IngredientEntity>()
                .Property(x => x.Fat)
                .HasPrecision(18, 2);
        }
    }
    
}