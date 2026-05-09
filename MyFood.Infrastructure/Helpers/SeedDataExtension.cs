using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyFood.Api.Services;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Infrastructure.Helpers
{
    public static class SeedDataExtension
    {
        public static void SeedData(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FoodDbContext>();
                var seedDataService = scope.ServiceProvider.GetRequiredService<ISeedDataService>();

                if (dbContext.Database.IsRelational())
                {
                    dbContext.Database.EnsureCreated();
                    dbContext.Database.ExecuteSqlRaw(@"
                        IF COL_LENGTH('FoodItems', 'Protein') IS NULL ALTER TABLE FoodItems ADD Protein float NOT NULL CONSTRAINT DF_FoodItems_Protein DEFAULT 0;
                        IF COL_LENGTH('FoodItems', 'Carbs') IS NULL ALTER TABLE FoodItems ADD Carbs float NOT NULL CONSTRAINT DF_FoodItems_Carbs DEFAULT 0;
                        IF COL_LENGTH('FoodItems', 'Fat') IS NULL ALTER TABLE FoodItems ADD Fat float NOT NULL CONSTRAINT DF_FoodItems_Fat DEFAULT 0;
                        IF COL_LENGTH('Ingredients', 'Unit') IS NULL ALTER TABLE Ingredients ADD Unit nvarchar(50) NULL;
                        IF COL_LENGTH('Ingredients', 'CaloriesPerUnit') IS NULL ALTER TABLE Ingredients ADD CaloriesPerUnit float NOT NULL CONSTRAINT DF_Ingredients_CaloriesPerUnit DEFAULT 0;
                        IF COL_LENGTH('Ingredients', 'Protein') IS NULL ALTER TABLE Ingredients ADD Protein float NOT NULL CONSTRAINT DF_Ingredients_Protein DEFAULT 0;
                        IF COL_LENGTH('Ingredients', 'Carbs') IS NULL ALTER TABLE Ingredients ADD Carbs float NOT NULL CONSTRAINT DF_Ingredients_Carbs DEFAULT 0;
                        IF COL_LENGTH('Ingredients', 'Fat') IS NULL ALTER TABLE Ingredients ADD Fat float NOT NULL CONSTRAINT DF_Ingredients_Fat DEFAULT 0;
                    ");
                }

                if (!dbContext.FoodItems.Any())
                {
                    seedDataService.Initialize(dbContext);
                }
            }
        }
    }
}
