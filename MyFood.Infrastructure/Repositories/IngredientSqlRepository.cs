

using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using MyFood.Infrastructure.Helpers;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _DbContext;

        public IngredientSqlRepository(FoodDbContext DbContext)
        {
            _DbContext = DbContext;
        }

        public async Task<IngredientEntity?> GetSingle(int id)
        {
            return await _DbContext.Ingredients.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IngredientEntity> Add(IngredientEntity item)
        {
            _DbContext.Ingredients.Add(item);
            await _DbContext.SaveChangesAsync();
            return item;
        }

        public async Task<IngredientEntity?> Delete(int id)
        {
            var ingredientItem = await GetSingle(id);
            if (ingredientItem != null)
            {
                _DbContext.Ingredients.Remove(ingredientItem);
                await _DbContext.SaveChangesAsync();
            }
            return ingredientItem;
        }

        public async Task<IngredientEntity> Update(int id, IngredientEntity item)
        {
            var existingItem = await _DbContext.Ingredients.FindAsync(id);
            if (existingItem != null)
            {
                _DbContext.Entry(existingItem).CurrentValues.SetValues(item);
                await _DbContext.SaveChangesAsync();
            }
            return item;
        }

        public async Task<IEnumerable<IngredientEntity>> GetAll(QueryParameters queryParameters)
        {
            return await _DbContext.Ingredients
                .OrderBy(x => x.Name)
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount)
                .ToListAsync();
        }

        public async Task<int> Count()
        {
            return await _DbContext.Ingredients.CountAsync();
        }
    }
}
