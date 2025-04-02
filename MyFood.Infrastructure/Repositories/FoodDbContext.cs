using Microsoft.EntityFrameworkCore;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Models; // Ensure this is the correct namespace for FoodItem

public class FoodDbContext : DbContext
{
    public FoodDbContext(DbContextOptions<FoodDbContext> options) : base(options)
    {
    }

    public DbSet<FoodEntity> FoodItems { get; set; } // Make sure this exists
}
