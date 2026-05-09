using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFood.Domain.Entities;  // ? This now contains ALL entities (ApplicationUser, FoodEntity, IngredientEntity)

namespace MyFood.Infrastructure.Repositories
{
    public class FoodDbContext : IdentityDbContext<ApplicationUser>
    {
        public FoodDbContext(DbContextOptions<FoodDbContext> options) : base(options)
        {
        }

        public DbSet<FoodEntity> FoodItems { get; set; }
        public DbSet<IngredientEntity> Ingredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Your entity configurations
            modelBuilder.Entity<FoodEntity>()
                .HasMany(f => f.Ingredients)
                .WithOne(i => i.FoodItem)
                .HasForeignKey(i => i.FoodId);
        }
    }
    
}