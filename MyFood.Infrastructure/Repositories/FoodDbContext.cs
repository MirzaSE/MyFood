<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
=======
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
>>>>>>> a138ff7 (Add JWT authentication setup)
using Microsoft.EntityFrameworkCore;
=======
﻿  using Microsoft.EntityFrameworkCore;
>>>>>>> b105ad4 (Week Three App)
=======
using Microsoft.EntityFrameworkCore;
>>>>>>> d243566 (Assignment 1 - Ingredients API)
using MyFood.Application.Entities;
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
<<<<<<< HEAD
        public DbSet<IngredientEntity> Ingredients { get; set; } = null!; 
=======
        public DbSet<IngredientEntity> Ingredients { get; set; } = null!;
>>>>>>> d243566 (Assignment 1 - Ingredients API)

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

<<<<<<< HEAD
            modelBuilder.Entity<FoodEntity>()
                .HasMany(f => f.Ingredients)
                .WithOne(i => i.FoodEntity)
                .HasForeignKey(i => i.FoodEntityId)
=======
            modelBuilder.Entity<IngredientEntity>()
                .HasOne(i => i.Food)
                .WithMany(f => f.Ingredients)
                .HasForeignKey(i => i.FoodId)
>>>>>>> d243566 (Assignment 1 - Ingredients API)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}