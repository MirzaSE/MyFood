  using Microsoft.EntityFrameworkCore;
using MyFood.Application.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class FoodDbContext : IdentityDbContext<ApplicationUser>
    {
        public FoodDbContext(DbContextOptions<FoodDbContext> options)
            : base(options)
        {
        }

        public DbSet<IngredientEntity> Ingredients { get; set; }
        public DbSet<FoodEntity> FoodItems { get; set; } = null!;
    }
}