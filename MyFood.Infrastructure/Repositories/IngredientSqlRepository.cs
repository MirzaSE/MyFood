using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _dbContext;

        public IngredientSqlRepository(FoodDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<IngredientEntity>> GetAllAsync(QueryParameters queryParameters)
        {
            return await _dbContext.Ingredients
                .Skip((queryParameters.Page - 1) * queryParameters.PageCount)
                .Take(queryParameters.PageCount)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.Ingredients.CountAsync();
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
            var existing = await _dbContext.Ingredients.FindAsync(id);
            if (existing == null) return null;

            if (ingredient.Name != null) existing.Name = ingredient.Name;
            if (ingredient.Unit != null) existing.Unit = ingredient.Unit;
            if (ingredient.CaloriesPerUnit != 0) existing.CaloriesPerUnit = ingredient.CaloriesPerUnit;
            if (ingredient.Protein != 0) existing.Protein = ingredient.Protein;
            if (ingredient.Carbs != 0) existing.Carbs = ingredient.Carbs;
            if (ingredient.Fat != 0) existing.Fat = ingredient.Fat;
            if (ingredient.Quantity != 0) existing.Quantity = ingredient.Quantity;
            if (ingredient.FoodEntityId.HasValue) existing.FoodEntityId = ingredient.FoodEntityId;

            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ingredient = await _dbContext.Ingredients.FindAsync(id);
            if (ingredient == null) return false;

            _dbContext.Ingredients.Remove(ingredient);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<IngredientEntity>> SearchAsync(string name)
        {
            return await _dbContext.Ingredients
                .Where(i => i.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string name)
        {
            return await _dbContext.Ingredients
                .AnyAsync(i => i.Name.ToLower() == name.ToLower());
        }
    }
}
