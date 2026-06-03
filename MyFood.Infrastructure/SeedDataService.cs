using MyFood.Application.Entities;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Api.Services
{
    public class SeedDataService : ISeedDataService
    {
        public void Initialize(FoodDbContext context)
        {
            if (context.FoodItems.Any())
            {
                return;
            }

            context.FoodItems.AddRange(
                new FoodEntity
                {
                    Calories = 1000,
                    Type = "Starter",
                    Name = "Lasagne",
                    Created = DateTime.Now,
                    Ingredients =
                    {
                        new IngredientEntity { Name = "Pasta sheets", Quantity = "6 pieces" },
                        new IngredientEntity { Name = "Cheese", Quantity = "200 g" }
                    }
                },
                new FoodEntity
                {
                    Calories = 1100,
                    Type = "Main",
                    Name = "Hamburger",
                    Created = DateTime.Now,
                    Ingredients =
                    {
                        new IngredientEntity { Name = "Beef patty", Quantity = "1 piece" },
                        new IngredientEntity { Name = "Burger bun", Quantity = "1 piece" }
                    }
                },
                new FoodEntity
                {
                    Calories = 1200,
                    Type = "Dessert",
                    Name = "Spaghetti",
                    Created = DateTime.Now,
                    Ingredients =
                    {
                        new IngredientEntity { Name = "Spaghetti", Quantity = "250 g" },
                        new IngredientEntity { Name = "Tomato sauce", Quantity = "150 ml" }
                    }
                },
                new FoodEntity
                {
                    Calories = 1300,
                    Type = "Starter",
                    Name = "Pizza",
                    Created = DateTime.Now,
                    Ingredients =
                    {
                        new IngredientEntity { Name = "Pizza dough", Quantity = "1 piece" },
                        new IngredientEntity { Name = "Mozzarella", Quantity = "180 g" }
                    }
                });

            context.SaveChanges();
        }
    }
}
