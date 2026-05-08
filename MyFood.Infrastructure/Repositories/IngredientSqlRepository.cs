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

        public IngredientEntity GetSingle(int id)
        {
            return _foodDbContext.Ingredients.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<IngredientEntity> GetAll()
        {
            return _foodDbContext.Ingredients.ToList();
        }

        public IEnumerable<IngredientEntity> GetByFoodId(int foodId)
        {
            return _foodDbContext.Ingredients.Where(x => x.FoodEntityId == foodId).ToList();
        }

        public void Add(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Add(item);
        }

        public void Delete(int id)
        {
            IngredientEntity ingredient = GetSingle(id);
            _foodDbContext.Ingredients.Remove(ingredient);
        }

        public IngredientEntity Update(int id, IngredientEntity item)
        {
            _foodDbContext.Ingredients.Update(item);
            return item;
        }

        public bool Save()
        {
            return (_foodDbContext.SaveChanges() >= 0);
        }
    }
}