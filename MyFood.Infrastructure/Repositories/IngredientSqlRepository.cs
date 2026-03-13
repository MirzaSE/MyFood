using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Helpers;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _dbContext;

        // Constructor uses your FoodDbContext
        public IngredientSqlRepository(FoodDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<IngredientEntity>> GetAllAsync()
        {
            return await _dbContext.Ingredients.ToListAsync();
        }

        public async Task<IngredientEntity?> GetByIdAsync(int id)
        {
            return await _dbContext.Ingredients.FindAsync(id);
        }

        public async Task<IngredientEntity> AddAsync(IngredientEntity ingredient)
        {
            _dbContext.Ingredients.Add(ingredient);
            await _dbContext.SaveChangesAsync();
            return ingredient;
        }

        public async Task<IngredientEntity?> UpdateAsync(int id, IngredientEntity ingredient)
        {
            var existingIngredient = await _dbContext.Ingredients.FindAsync(id);
            if (existingIngredient == null) return null;

            existingIngredient.Name = ingredient.Name;
            existingIngredient.Quantity = ingredient.Quantity;

            await _dbContext.SaveChangesAsync();
            return existingIngredient;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ingredient = await _dbContext.Ingredients.FindAsync(id);
            if (ingredient == null) return false;

            _dbContext.Ingredients.Remove(ingredient);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}