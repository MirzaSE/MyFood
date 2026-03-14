
using MyFood.Application.Entities;
using System.Linq;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _dbContext;
        public IngredientSqlRepository(FoodDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IQueryable<IngredientEntity> GetAll()
        {
            return _dbContext.Ingredients;
        }

        public IngredientEntity? GetSingle(int id)
        {
            return _dbContext.Ingredients.FirstOrDefault(i => i.Id == id);
        }
        public void Add(IngredientEntity item)
        {
            _dbContext.Ingredients.Add(item);
        }
        public void Delete(IngredientEntity item)
        {
            _dbContext.Ingredients.Remove(item);
        }
        public IngredientEntity Update(IngredientEntity item)
        {
            _dbContext.Ingredients.Update(item);
            return item;
        }
        public bool Save()
        {
            return (_dbContext.SaveChanges() >= 0);
        }
    }
}
