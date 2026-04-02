using Microsoft.EntityFrameworkCore;
using MyFood.Domain.Entities;
using MyFood.Application.Repositories;

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
            return await _context.Ingredients.FindAsync(id);
        }

        public async Task<IngredientEntity> AddAsync(IngredientEntity ingredient)
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();

            return ingredient;
        }

        public async Task<IngredientEntity?> UpdateAsync(int id, IngredientEntity ingredient)
        {
            var existing = await _context.Ingredients.FindAsync(id);

            if (existing == null)
                return null;

            existing.Name = ingredient.Name;
            existing.Quantity = ingredient.Quantity;
            existing.FoodId = ingredient.FoodId;

            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);

            if (ingredient == null)
                return false;

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
