using MyFood.Application.Entities;
using MyFood.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _context;

        public IngredientSqlRepository(FoodDbContext context)
        {
            _context = context;
        }

        public void AddIngredient(IngredientEntity ingredient)
        {
            _context.Ingredients.Add(ingredient);
            _context.SaveChanges();
        }

        public void UpdateIngredient(IngredientEntity ingredient)
        {
            _context.Ingredients.Update(ingredient);
            _context.SaveChanges();
        }

        public void DeleteIngredient(int ingredientId)
        {
            var ingredient = _context.Ingredients.Find(ingredientId);
            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
                _context.SaveChanges();
            }
        }

        public List<IngredientEntity> GetAllIngredients()
        {
            return _context.Ingredients.ToList();
        }

        public List<IngredientEntity> GetIngredientsByFoodId(int foodId)
        {
            return _context.Ingredients
                .Where(i => i.FoodEntityId == foodId)
                .ToList();
        }
    }
}
