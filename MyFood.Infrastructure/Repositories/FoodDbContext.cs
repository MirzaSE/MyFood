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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FoodEntity>()
                .HasMany(f => f.Ingredients)
                .WithOne(i => i.FoodEntity)
                .HasForeignKey(i => i.FoodEntityId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IngredientEntity>()
                .Property(i => i.FoodEntityId)
                .HasColumnName("FoodId");

            modelBuilder.Entity<IngredientEntity>()
                .Property(i => i.CaloriesPerUnit)
                .HasPrecision(10, 2);

            modelBuilder.Entity<IngredientEntity>()
                .Property(i => i.Protein)
                .HasPrecision(10, 2);

            modelBuilder.Entity<IngredientEntity>()
                .Property(i => i.Carbs)
                .HasPrecision(10, 2);

            modelBuilder.Entity<IngredientEntity>()
                .Property(i => i.Fat)
                .HasPrecision(10, 2);
        }
    }
    
}
