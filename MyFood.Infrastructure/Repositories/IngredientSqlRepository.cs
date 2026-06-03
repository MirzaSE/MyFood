using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<IngredientEntity> GetAll()
        {
            return _foodDbContext.Ingredients
                .Include(i => i.Food)
                .OrderBy(i => i.Name)
                .ToList();
        }

        public IngredientEntity? GetSingle(int id)
        {
            return _foodDbContext.Ingredients
                .Include(i => i.Food)
                .FirstOrDefault(i => i.Id == id);
        }

        public IEnumerable<IngredientEntity> GetByFoodId(int foodId)
        {
            return _foodDbContext.Ingredients
                .Where(i => i.FoodId == foodId)
                .OrderBy(i => i.Name)
                .ToList();
        }

        public void Add(IngredientEntity ingredient)
        {
            _foodDbContext.Ingredients.Add(ingredient);
        }

        public IngredientEntity Update(int id, IngredientEntity ingredient)
        {
            var existingIngredient = GetSingle(id);

            if (existingIngredient == null)
            {
                throw new InvalidOperationException($"Ingredient with id {id} was not found.");
            }

            existingIngredient.Name = ingredient.Name;
            existingIngredient.Quantity = ingredient.Quantity;
            existingIngredient.FoodId = ingredient.FoodId;

            return existingIngredient;
        }

        public void Delete(int id)
        {
            var ingredient = GetSingle(id);

            if (ingredient != null)
            {
                _foodDbContext.Ingredients.Remove(ingredient);
            }
        }

        public bool Save()
        {
            return (_foodDbContext.SaveChanges() >= 0);
        }
    }
}
