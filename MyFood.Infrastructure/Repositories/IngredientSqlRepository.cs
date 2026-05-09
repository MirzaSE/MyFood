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

        public IngredientEntity GetSingle(int id)
        {
            return _foodDbContext.Ingredients.FirstOrDefault(x => x.Id == id);
        }

        public void Add(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Add(item);
        }

        public void Delete(int id)
        {
            IngredientEntity ingredientItem = GetSingle(id);
            _foodDbContext.Ingredients.Remove(ingredientItem);
        }

        public IngredientEntity Update(int id, IngredientEntity item)
        {
            _foodDbContext.Ingredients.Update(item);
            return item;
        }

        public IQueryable<IngredientEntity> GetAll()
        {
            return _foodDbContext.Ingredients.OrderBy(x => x.Name);
        }

        public IEnumerable<IngredientEntity> GetByFoodId(int foodEntityId)
        {
            return _foodDbContext.Ingredients
                .Where(x => x.FoodEntityId == foodEntityId)
                .OrderBy(x => x.Name)
                .ToList();
        }

        public IEnumerable<IngredientEntity> SearchIngredientsByName(string name)
        {
            IQueryable<IngredientEntity> ingredients = _foodDbContext.Ingredients.OrderBy(x => x.Name);

            if (string.IsNullOrWhiteSpace(name))
            {
                return ingredients.ToList();
            }

            return ingredients
                .Where(x => x.Name != null && EF.Functions.Like(x.Name, $"%{name}%"))
                .ToList();
        }

        public int Count()
        {
            return _foodDbContext.Ingredients.Count();
        }

        public bool Save()
        {
            return (_foodDbContext.SaveChanges() >= 0);
        }
    }
}