using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyFood.Api.Data.Entities;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Api.Data.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _context;

        public IngredientSqlRepository(FoodDbContext context)
        {
            _context = context;
        }

        public async Task<IngredientEntity> GetByIdAsync(int id)
        {
            return await _context.Ingredients.FindAsync(id);
        }

        public async Task<IEnumerable<IngredientEntity>> GetAllAsync()
        {
            return await _context.Ingredients.ToListAsync();
        }

        public async Task AddAsync(IngredientEntity ingredient)
        {
            await _context.Ingredients.AddAsync(ingredient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(IngredientEntity ingredient)
        {
            _context.Ingredients.Update(ingredient);
            await _context.SaveChangesAsync();
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
    }
}