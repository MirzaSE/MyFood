using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFood.Application.Entities;
using MyFood.Domain.Entities;

public class FoodDbContext : IdentityDbContext<ApplicationUser>
{
    public FoodDbContext(DbContextOptions<FoodDbContext> options) : base(options) { }

    public DbSet<FoodEntity> FoodItems { get; set; }
}
