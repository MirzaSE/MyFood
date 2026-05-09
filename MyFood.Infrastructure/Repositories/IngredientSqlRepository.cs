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

        public IngredientEntity? GetSingle(int id)
        {
            return _foodDbContext.Ingredients
                .Include(x => x.Food)
                .FirstOrDefault(x => x.Id == id);
            //return _foodDbContext.FoodItems.FirstOrDefault(x => x.Id == id);
        }

        public void Add(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Add(item);
            //_foodDbContext.FoodItems.Add(item);
        }

        public void Delete(int id)
        {
            IngredientEntity? ingredient = GetSingle(id);
            if (ingredient == null)
            {
                return;
            }
            _foodDbContext.Ingredients.Remove(ingredient);
            //FoodEntity foodItem = GetSingle(id);
            // _foodDbContext.FoodItems.Remove(foodItem);
        }

        public IngredientEntity Update(int id, IngredientEntity item)
        {
            _foodDbContext.Ingredients.Update(item);
            //_foodDbContext.FoodItems.Update(item);
            return item;
        }

        public IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<IngredientEntity> _allItems = _foodDbContext.Ingredients
                .Include(x => x.Food)
                .OrderBy(x => x.Name);

            if (queryParameters.HasQuery())
            {
                _allItems = _allItems
                    .Where(x => x.Name.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant()));
            }

            return _allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public int Count()
        {
            return _foodDbContext.Ingredients.Count();
            //return _foodDbContext.FoodItems.Count();
        }

        public bool Save()
        {
            return (_foodDbContext.SaveChanges() >= 0);
        }


        public IEnumerable<IngredientEntity> SearchFoodsByName(string name)
        {
            return _foodDbContext.Ingredients
                .Where(f => EF.Functions.Like(f.Name, $"%{name}%"))
                .ToList();

            // SELECT * FROM FoodItems WHERE Name LIKE '%name%'
        }
    }
}
