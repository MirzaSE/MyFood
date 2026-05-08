<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFood.Domain.Entities;
=======
﻿using Microsoft.EntityFrameworkCore;
using MyFood.Application.Entities;
>>>>>>> origin/spring2026/assignment1/almir.bajric/220302201

namespace MyFood.Infrastructure.Repositories
{
    public class FoodDbContext : IdentityDbContext<ApplicationUser>
    {
        public FoodDbContext(DbContextOptions<FoodDbContext> options)
            : base(options)
        {
        }

        public DbSet<FoodEntity> FoodItems { get; set; } = null!;
<<<<<<< HEAD
        public DbSet<IngredientEntity> Ingredients { get; set; } = null!; 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FoodEntity>()
                .HasMany(f => f.Ingredients)
                .WithOne(i => i.FoodEntity)
                .HasForeignKey(i => i.FoodEntityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    
=======
        public DbSet<IngredientEntity> Ingredients { get; set; } = null!;
    }   
>>>>>>> origin/spring2026/assignment1/almir.bajric/220302201
}