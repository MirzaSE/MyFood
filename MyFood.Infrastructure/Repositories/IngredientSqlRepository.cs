using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _context;

        public IngredientSqlRepository(FoodDbContext context)
        {
            _context = context;
        }

        public IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<IngredientEntity> items = _context.Ingredients.OrderBy(x => x.Name);

            if (!string.IsNullOrWhiteSpace(queryParameters.Query))
            {
                var q = queryParameters.Query.ToLower();
                items = items.Where(x => x.Name.ToLower().Contains(q));
            }

            return items
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public async Task<IngredientEntity?> GetByIdAsync(int id)
        {
            return await _context.Ingredients.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IngredientEntity> AddAsync(IngredientEntity ingredient)
        {
            await _context.Ingredients.AddAsync(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task<IngredientEntity?> UpdateAsync(int id, IngredientEntity ingredient)
        {
            var existing = await _context.Ingredients.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return null;

            existing.Name = ingredient.Name;
            existing.Unit = ingredient.Unit;
            existing.CaloriesPerUnit = ingredient.CaloriesPerUnit;
            existing.Protein = ingredient.Protein;
            existing.Carbs = ingredient.Carbs;
            existing.Fat = ingredient.Fat;
            existing.FoodEntityId = ingredient.FoodEntityId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ingredient = await _context.Ingredients.FirstOrDefaultAsync(x => x.Id == id);
            if (ingredient == null) return false;

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<IngredientEntity>> SearchAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return await _context.Ingredients.OrderBy(x => x.Name).ToListAsync();

            return await _context.Ingredients
                .Where(x => EF.Functions.Like(x.Name, $"%{name}%"))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Ingredients.CountAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            var lower = name.ToLower();
            var query = _context.Ingredients.Where(x => x.Name.ToLower() == lower);
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}
