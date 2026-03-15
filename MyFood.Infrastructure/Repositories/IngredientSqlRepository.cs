using Microsoft.EntityFrameworkCore;
using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _context;

        public IngredientSqlRepository(FoodDbContext context)
        {
            _context = context;
        }

        public async Task AddIngredientAsync(IngredientEntity ingredient)
        {
            await _context.Ingredients.AddAsync(ingredient);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteIngredientAsync(IngredientEntity ingredient)
        {
            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
        }

        public async Task<IngredientEntity?> GetIngredientByIdAsync(int id)
        {
            return await _context.Ingredients.FindAsync(id);
        }

        public async Task<IEnumerable<IngredientEntity>> ListIngredientsAsync()
        {
            return await _context.Ingredients.ToListAsync();
        }

        public async Task UpdateIngredientAsync(IngredientEntity ingredient)
        {
            _context.Ingredients.Update(ingredient);
            await _context.SaveChangesAsync();
        }
    }
}