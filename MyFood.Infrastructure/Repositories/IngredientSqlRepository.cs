using Microsoft.EntityFrameworkCore;
using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _foodDbContext;

        public IngredientSqlRepository(FoodDbContext foodDbContext)
        {
            _foodDbContext = foodDbContext;
        }

        public IngredientEntity GetSingle(int id)
        {
            return _foodDbContext.Ingredients
                .FirstOrDefault(x => x.Id == id);
        }
        public IEnumerable<IngredientEntity> GetAllByFoodId(int foodId)
        {
            return _foodDbContext.Ingredients
                .Where(x => x.FoodId == foodId)
                .OrderBy(x => x.Name)
                .ToList();
        }
        public void Add(int foodId, IngredientEntity ingredient)
        {
            ingredient.FoodId = foodId;
            _foodDbContext.Ingredients.Add(ingredient);
        }

        public IngredientEntity Update(int id, IngredientEntity ingredient)
        {
            ingredient.Id = id;
            _foodDbContext.Ingredients.Update(ingredient);
            return ingredient;
        }

        public void Delete(int id)
        {
            IngredientEntity ingredient = GetSingle(id);

            if (ingredient == null)
            {
                throw new InvalidOperationException($"Ingredient with id {id} was not found.");
            }

            _foodDbContext.Ingredients.Remove(ingredient);
        }

        public bool FoodExists(int foodId)
        {
            return _foodDbContext.FoodItems.Any(x => x.Id == foodId);
        }
        public int Count(int foodId)
        {
            return _foodDbContext.Ingredients
                .Count(x => x.FoodId == foodId);
        }

        public bool Save()
        {
            return (_foodDbContext.SaveChanges() >= 0);
        }
    }
}