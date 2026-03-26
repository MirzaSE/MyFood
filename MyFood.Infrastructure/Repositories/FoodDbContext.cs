using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFood.Application.Entities;       // FoodEntity, IngredientEntity
using MyFood.Infrastructure.Entities;    // ApplicationUser

namespace MyFood.Infrastructure.Repositories
{
    public class FoodDbContext : IdentityDbContext<ApplicationUser>
    {
        public FoodDbContext(DbContextOptions<FoodDbContext> options)
            : base(options)
        {
            Console.WriteLine(this.Database.GetConnectionString());
        }

        public DbSet<FoodEntity> FoodItems { get; set; } = null!;
        public DbSet<IngredientEntity> Ingredients { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Important for Identity tables

            modelBuilder.Entity<IngredientEntity>()
                .HasOne(i => i.foodItem)
                .WithMany(f => f.Ingredients)
                .HasForeignKey(i => i.Food_Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}