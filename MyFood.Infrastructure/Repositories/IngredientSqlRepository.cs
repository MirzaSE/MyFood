using Microsoft.EntityFrameworkCore;
using MyFood.Application.Entities;
using MyFood.Application.Interfaces;
using MyFood.Infrastructure;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _context;

        public IngredientSqlRepository(FoodDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<IngredientEntity>> GetAllAsync()
        {
            return await _context.Ingredients.ToListAsync();
        }

        public async Task<IngredientEntity?> GetByIdAsync(int id)
        {
            return await _context.Ingredients
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IngredientEntity> AddAsync(IngredientEntity ingredient)
        {
            await _context.Ingredients.AddAsync(ingredient);
            await _context.SaveChangesAsync();

            return ingredient;
        }

        public async Task<IngredientEntity?> UpdateAsync(int id, IngredientEntity ingredient)
        {
            var existingIngredient = await _context.Ingredients
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingIngredient == null)
                return null;

            existingIngredient.Name = ingredient.Name;
            existingIngredient.Quantity = ingredient.Quantity;
            existingIngredient.FoodId = ingredient.FoodId;

            await _context.SaveChangesAsync();

            return existingIngredient;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(x => x.Id == id);

            if (ingredient == null)
                return false;

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}