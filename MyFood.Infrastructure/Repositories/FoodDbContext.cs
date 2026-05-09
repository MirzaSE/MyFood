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
                .HasOne(fi => fi.FoodEntity)
                .WithMany(f => f.FoodIngredients)
                .HasForeignKey(fi => fi.FoodEntityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FoodIngredientEntity>()
                .HasOne(fi => fi.Ingredient)
                .WithMany(i => i.FoodIngredients)
                .HasForeignKey(fi => fi.IngredientEntityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
