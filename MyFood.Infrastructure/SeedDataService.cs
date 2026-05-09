using MyFood.Domain.Entities;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Api.Services
{
    public class SeedDataService : ISeedDataService
    {
        public void Initialize(FoodDbContext context)
        {
            if (context.FoodItems.Any() || context.Ingredients.Any())
            {
                return;
            }

            context.Ingredients.AddRange(
                new IngredientEntity { Name = "Chicken Breast", Unit = "100g", CaloriesPerUnit = 165, Protein = 31, Carbs = 0, Fat = 3.6m },
                new IngredientEntity { Name = "Rice", Unit = "100g", CaloriesPerUnit = 130, Protein = 2.7m, Carbs = 28, Fat = 0.3m },
                new IngredientEntity { Name = "Olive Oil", Unit = "tbsp", CaloriesPerUnit = 119, Protein = 0, Carbs = 0, Fat = 13.5m },
                new IngredientEntity { Name = "Tomato", Unit = "100g", CaloriesPerUnit = 18, Protein = 0.9m, Carbs = 3.9m, Fat = 0.2m }
            );

            context.FoodItems.Add(new FoodEntity() { Calories = 1000, Type = "Starter", Name = "Lasagne", Created = DateTime.UtcNow });
            context.FoodItems.Add(new FoodEntity() { Calories = 1100, Type = "Main", Name = "Hamburger", Created = DateTime.UtcNow });
            context.FoodItems.Add(new FoodEntity() { Calories = 1200, Type = "Dessert", Name = "Spaghetti", Created = DateTime.UtcNow });
            context.FoodItems.Add(new FoodEntity() { Calories = 1300, Type = "Starter", Name = "Pizza", Created = DateTime.UtcNow });

            context.SaveChanges();
        }
    }
}
