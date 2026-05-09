using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly FoodDbContext _context;

        public IngredientRepository(FoodDbContext context)
        {
            _context = context;
        }

        public IQueryable<IngredientEntity> GetAll(QueryParameters parameters)
        {
            return _context.Ingredients
                .OrderBy(i => i.Name)
                .Skip((parameters.Page - 1) * parameters.PageCount)
                .Take(parameters.PageCount);
        }

        public IngredientEntity? GetSingle(int id)
        {
            return _context.Ingredients.FirstOrDefault(i => i.Id == id);
        }

        public async Task<IngredientEntity?> GetByNameAsync(string name)
        {
            return await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());
        }

        public async Task<IngredientEntity> CreateAsync(IngredientEntity ingredient)
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task<IngredientEntity> UpdateAsync(IngredientEntity ingredient)
        {
            _context.Ingredients.Update(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ingredient = await _context.Ingredients.FirstOrDefaultAsync(i => i.Id == id);
            if (ingredient == null)
                return false;

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<IngredientEntity>> SearchAsync(string searchTerm)
        {
            return await _context.Ingredients
                .Where(i => i.Name.ToLower().Contains(searchTerm.ToLower()))
                .OrderBy(i => i.Name)
                .ToListAsync();
        }
    }
}
