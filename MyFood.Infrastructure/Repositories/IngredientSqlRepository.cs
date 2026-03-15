

using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Entities;
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
            return await _DbContext.IngredientItems.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IngredientEntity> Add(IngredientEntity item)
        {
            _DbContext.IngredientItems.Add(item);
            await _DbContext.SaveChangesAsync();
            return item;
        }

        public async Task<IngredientEntity?> Delete(int id)
        {
            var ingredientItem = await GetSingle(id);
            if (ingredientItem != null)
            {
                _DbContext.IngredientItems.Remove(ingredientItem);
                await _DbContext.SaveChangesAsync();
            }
            return ingredientItem;
        }

        public async Task<IngredientEntity> Update(int id, IngredientEntity item)
        {
            var existingItem = await _DbContext.IngredientItems.FindAsync(id);
            if (existingItem != null)
            {
                _DbContext.Entry(existingItem).CurrentValues.SetValues(item);
                await _DbContext.SaveChangesAsync();
            }
            return item;
        }

        public async Task<IEnumerable<IngredientEntity>> GetAll(QueryParameters queryParameters)
        {
            return await _DbContext.IngredientItems
                .OrderBy(x => x.Name)
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount)
                .ToListAsync();
        }

        public async Task<int> Count()
        {
            return await _DbContext.IngredientItems.CountAsync();
        }
    }
}
