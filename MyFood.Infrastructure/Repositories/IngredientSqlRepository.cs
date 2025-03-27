using Microsoft.EntityFrameworkCore;
using MyFood.Application.Entities;
using MyFood.Application.Repositories;
using MyFood.Domain.Entities;
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

        // Add a new ingredient
        public async Task<IngredientEntity> AddAsync(IngredientEntity ingredient)
        {
            await _context.Ingredients.AddAsync(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        // Update an existing ingredient
        public async Task<IngredientEntity> UpdateAsync(IngredientEntity ingredient)
        {
            var existingIngredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Id == ingredient.Id);

            if (existingIngredient == null)
                throw new KeyNotFoundException("Ingredient not found");

            existingIngredient.Name = ingredient.Name;
            existingIngredient.Quantity = ingredient.Quantity;

            await _context.SaveChangesAsync();
            return existingIngredient;
        }

        // Delete an ingredient by ID
        public async Task<bool> DeleteAsync(int ingredientId)
        {
            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Id == ingredientId);

            if (ingredient == null)
                return false;

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get an ingredient by ID
        public async Task<IngredientEntity> GetByIdAsync(int ingredientId)
        {
            return await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Id == ingredientId);
        }

        // Get all ingredients
        public async Task<IEnumerable<IngredientEntity>> GetAllAsync()
        {
            return await _context.Ingredients.ToListAsync();
        }

        // Get ingredients by name (optional)
        public async Task<IEnumerable<IngredientEntity>> GetByNameAsync(string name)
        {
            return await _context.Ingredients
                .Where(i => i.Name.Contains(name))
                .ToListAsync();
        }
    }
}

