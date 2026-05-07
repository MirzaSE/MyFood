using Microsoft.EntityFrameworkCore;
using MyFood.Application.Interfaces;
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

        public async Task<IEnumerable<IngredientEntity>> GetAllAsync() =>
            await _context.Ingredients.AsNoTracking().ToListAsync();

        public Task<IngredientEntity?> GetByIdAsync(int id) =>
            _context.Ingredients.FirstOrDefaultAsync(i => i.Id == id);

        public Task<bool> ExistsByNameAsync(string name) =>
            _context.Ingredients.AnyAsync(i => i.Name.ToLower() == name.ToLower());

        public Task<bool> ExistsByNameExceptIdAsync(string name, int exceptId) =>
            _context.Ingredients.AnyAsync(i =>
                i.Name.ToLower() == name.ToLower() && i.Id != exceptId);

        public async Task<IngredientEntity> CreateAsync(IngredientEntity ingredient)
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task UpdateAsync(IngredientEntity ingredient)
        {
            var existing = await _context.Ingredients.FindAsync([ingredient.Id]);
            if (existing != null)
            {
                existing.Name = ingredient.Name;
                existing.FoodEntityId = ingredient.FoodEntityId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync([id]);
            if (ingredient is null)
            {
                return false;
            }

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<IngredientEntity>> SearchAsync(string normalizedQueryLower)
        {
            if (string.IsNullOrWhiteSpace(normalizedQueryLower))
            {
                return await _context.Ingredients.AsNoTracking().ToListAsync();
            }

            var term = normalizedQueryLower.Trim();
            return await _context.Ingredients
                .AsNoTracking()
                .Where(i => i.Name.ToLower().Contains(term))
                .ToListAsync();
        }

        public void AddIngredient(IngredientEntity ingredient)
        {
            _context.Ingredients.Add(ingredient);
            _context.SaveChanges();
        }

        public IEnumerable<IngredientEntity> GetAllIngredients()
        {
            return _context.Ingredients.ToList();
        }

        public IngredientEntity? GetIngredientById(int id)
        {
            return _context.Ingredients.FirstOrDefault(i => i.Id == id);
        }

        public void UpdateIngredient(IngredientEntity ingredient)
        {
            var existing = _context.Ingredients.Find(ingredient.Id);
            if (existing != null)
            {
                existing.Name = ingredient.Name;
                existing.Unit = ingredient.Unit;
                existing.CaloriesPerUnit = ingredient.CaloriesPerUnit;
                existing.Protein = ingredient.Protein;
                existing.Carbs = ingredient.Carbs;
                existing.Fat = ingredient.Fat;
                existing.FoodEntityId = ingredient.FoodEntityId;
                _context.SaveChanges();
            }
        }

        public void DeleteIngredient(int id)
        {
            var ingredient = _context.Ingredients.Find(id);
            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
                _context.SaveChanges();
            }
        }
    }
}
