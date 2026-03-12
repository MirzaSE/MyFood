using MyFood.Domain.Entities;
using System.Linq;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _dbContext;

        // The database context is injected here via Dependency Injection
        public IngredientSqlRepository(FoodDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<IngredientEntity> GetAll()
        {
            // Returns all ingredients
            return _dbContext.Ingredients;
        }

        public IngredientEntity? GetSingle(int id)
        {
            // Finds a single ingredient by its ID
            return _dbContext.Ingredients.FirstOrDefault(i => i.Id == id);
        }

        public void Add(IngredientEntity item)
        {
            // Adds a new ingredient to the database tracking
            _dbContext.Ingredients.Add(item);
        }

        public void Delete(IngredientEntity item)
        {
            // Removes an ingredient from database tracking
            _dbContext.Ingredients.Remove(item);
        }

        public IngredientEntity Update(IngredientEntity item)
        {
            // Updates an existing ingredient
            _dbContext.Ingredients.Update(item);
            return item;
        }

        public bool Save()
        {
            // Saves the tracked changes to the actual SQL database
            return (_dbContext.SaveChanges() >= 0);
        }
    }
}