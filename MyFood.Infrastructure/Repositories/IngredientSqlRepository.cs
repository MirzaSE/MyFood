

using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Domain.Entities;
using MyFood.Infrastructure.Helpers;

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
            IngredientEntity Ingredients = GetSingle(id);
            _foodDbContext.Ingredients.Remove(Ingredients);
        }

        public IngredientEntity Update(int id, IngredientEntity item)
        {
            _foodDbContext.Ingredients.Update(item);
            return item;
        }

        public IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<IngredientEntity> _allItems = _foodDbContext.Ingredients.OrderBy(x=>x.Name);

            if (queryParameters.HasQuery())
            {
                _allItems = _allItems
                    .Where(x => x.Quantity.ToString().Contains(queryParameters.Query.ToLowerInvariant())
                    || x.Name.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant()));
            }

            return _allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public int Count()
        {
            return _foodDbContext.Ingredients.Count();
        }

        public bool Save()
        {
            return (_foodDbContext.SaveChanges() >= 0);
        }

        public ICollection<IngredientEntity> GetRandomMeal()
        {
            List<IngredientEntity> toReturn = new List<IngredientEntity>();

            toReturn.Add(GetRandomItem("Starter"));
            toReturn.Add(GetRandomItem("Main"));
            toReturn.Add(GetRandomItem("Dessert"));

            return toReturn;
        }


        public IEnumerable<IngredientEntity> SearchFoodsByName(string name)
        {
            return _foodDbContext.Ingredients
                .Where(f => EF.Functions.Like(f.Name, $"%{name}%"))
                .ToList();

            // SELECT * FROM FoodItems WHERE Name LIKE '%name%'
        }

        private IngredientEntity GetRandomItem(string type)
        {
            return _foodDbContext.Ingredients
                .Where(x => x.Name == type)
                .OrderBy(o => Guid.NewGuid())
                .FirstOrDefault();
        }
    }
}
