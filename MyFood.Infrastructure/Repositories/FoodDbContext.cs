 using Microsoft.EntityFrameworkCore;
using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class FoodDbContext : DbContext
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
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<IngredientEntity>()
        .HasOne(i => i.foodItem)
        .WithMany(f => f.Ingredients)
        .HasForeignKey(i => i.Food_Id)
        .OnDelete(DeleteBehavior.Cascade);
}
    }
}