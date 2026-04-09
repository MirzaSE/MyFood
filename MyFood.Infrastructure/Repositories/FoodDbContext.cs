using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Repositories.Models;

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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FoodEntity>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Name).IsRequired().HasMaxLength(100);
            });

            builder.Entity<IngredientEntity>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Name).IsRequired().HasMaxLength(100);
            });

        
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FullName).HasMaxLength(200);
                entity.Property(u => u.IsEmailVerified).HasDefaultValue(false);
                entity.Property(u => u.EmailVerificationToken).HasMaxLength(200).IsUnicode(false);
                entity.Property(u => u.EmailVerificationTokenExpiresAt);
            });
        }
    }
}