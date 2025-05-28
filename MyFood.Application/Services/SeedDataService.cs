using MyFood.Domain.Entities;
using MyFood.Domain.Interfaces;

namespace MyFood.Api.Services
{
    public class SeedDataService : ISeedDataService
    {
        public void Initialize(IFoodRepository repository)
        {
            repository.Add(new FoodEntity() { Calories = 1000, Type = "Starter", Name = "Lasagne", Created = DateTime.Now });
            repository.Add(new FoodEntity() { Calories = 1100, Type = "Main", Name = "Hamburger", Created = DateTime.Now });
            repository.Add(new FoodEntity() { Calories = 1200, Type = "Dessert", Name = "Spaghetti", Created = DateTime.Now });
            repository.Add(new FoodEntity() { Calories = 1300, Type = "Starter", Name = "Pizza", Created = DateTime.Now });

        }
    }
}
