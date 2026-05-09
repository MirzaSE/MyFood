using Microsoft.EntityFrameworkCore;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _foodDbContext;

        public IngredientSqlRepository(FoodDbContext foodDbContext)
        {
            _foodDbContext = foodDbContext;
        }

        public IQueryable<IngredientEntity> GetAll()
        {
            return _foodDbContext.Ingredients
                .Include(i => i.Food)
                .OrderBy(i => i.Name);
        }

        public IngredientEntity GetSingle(int id)
        {
            return _foodDbContext.Ingredients
                .Include(i => i.Food)
                .FirstOrDefault(i => i.Id == id);
        }

        public void Add(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Add(item);
        }

        public IngredientEntity Update(int id, IngredientEntity item)
        {
            _foodDbContext.Ingredients.Update(item);
            return item;
        }

        public void Delete(int id)
        {
            IngredientEntity ingredient = GetSingle(id);

            if (ingredient != null)
            {
                _foodDbContext.Ingredients.Remove(ingredient);
            }
        }

        public bool Save()
        {
            return _foodDbContext.SaveChanges() >= 0;
        }
    }
}
