using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyFood.Domain.Entities;
using MyFood.Domain.Repositories;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _dbContext;

        public IngredientSqlRepository(FoodDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<IngredientEntity>> GetAllAsync()
        {
            return await _dbContext.Ingredients.ToListAsync();
        }

        public async Task<IngredientEntity?> GetByIdAsync(int id)
        {
            return await _dbContext.Ingredients.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IngredientEntity> AddAsync(IngredientEntity ingredient)
        {
            _dbContext.Ingredients.Add(ingredient);
            await _dbContext.SaveChangesAsync();
            return ingredient;
        }

        public async Task<IngredientEntity> UpdateAsync(IngredientEntity ingredient)
        {
            _dbContext.Ingredients.Update(ingredient);
            await _dbContext.SaveChangesAsync();
            return ingredient;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbContext.Ingredients.FirstOrDefaultAsync(i => i.Id == id);
            if (entity != null)
            {
                _dbContext.Ingredients.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}