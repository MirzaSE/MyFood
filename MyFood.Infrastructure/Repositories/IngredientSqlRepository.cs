using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Domain.Entities;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _context;

        public IngredientSqlRepository(FoodDbContext context)
        {
            _context = context;
        }

        public async Task<IngredientEntity> AddAsync(IngredientEntity ingredient)
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task DeleteAsync(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<IngredientEntity>> GetAllByFoodIdAsync(int foodId)
        {
            return await _context.Ingredients
                .Where(i => i.FoodId == foodId)
                .ToListAsync();
        }

        public async Task<IngredientEntity?> GetByIdAsync(int id)
        {
            return await _context.Ingredients.FindAsync(id);
        }

        public async Task UpdateAsync(IngredientEntity ingredient)
        {
            _context.Entry(ingredient).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}