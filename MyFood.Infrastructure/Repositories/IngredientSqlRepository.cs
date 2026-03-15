using Microsoft.EntityFrameworkCore;
using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _foodDbContext;

        public IngredientSqlRepository(FoodDbContext foodDbContext)
        {
            _foodDbContext = foodDbContext;
        }

        public IEnumerable<IngredientEntity> GetAllForFood(int foodId)
        {
            return _foodDbContext.Ingredients
                .Where(x => x.FoodId == foodId)
                .OrderBy(x => x.Name)
                .AsNoTracking()
                .ToList();
        }

        public IngredientEntity? GetSingle(int foodId, int ingredientId)
        {
            return _foodDbContext.Ingredients
                .FirstOrDefault(x => x.FoodId == foodId && x.Id == ingredientId);
        }

        public void Add(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Add(item);
        }

        public void Update(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Update(item);
        }

        public void Delete(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Remove(item);
        }

        public bool Save()
        {
            return _foodDbContext.SaveChanges() >= 0;
        }
    }
}
