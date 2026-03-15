using MyFood.Domain.Entities;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Api.Services
{
    public class SeedDataService : ISeedDataService
    {
        public void Initialize(FoodDbContext context)
        {
            context.FoodItems.Add(new FoodEntity() { Calories = 1000, Type = "Starter", Name = "Lasagne", Created = DateTime.Now });
            context.FoodItems.Add(new FoodEntity() { Calories = 1100, Type = "Main", Name = "Hamburger", Created = DateTime.Now });
            context.FoodItems.Add(new FoodEntity() { Calories = 1200, Type = "Dessert", Name = "Spaghetti", Created = DateTime.Now });
            context.FoodItems.Add(new FoodEntity() { Calories = 1300, Type = "Starter", Name = "Pizza", Created = DateTime.Now });

            context.SaveChanges();

            // Seed ingredients for existing foods
            var lasagne = context.FoodItems.FirstOrDefault(f => f.Name == "Lasagne");
            if (lasagne != null)
            {
                context.Ingredients.Add(new IngredientEntity { Name = "Pasta", Quantity = "200g", FoodId = lasagne.Id });
                context.Ingredients.Add(new IngredientEntity { Name = "Cheese", Quantity = "100g", FoodId = lasagne.Id });
            }

            var hamburger = context.FoodItems.FirstOrDefault(f => f.Name == "Hamburger");
            if (hamburger != null)
            {
                context.Ingredients.Add(new IngredientEntity { Name = "Beef", Quantity = "150g", FoodId = hamburger.Id });
                context.Ingredients.Add(new IngredientEntity { Name = "Bun", Quantity = "1 piece", FoodId = hamburger.Id });
            }

            context.SaveChanges();
        }
    }
}
