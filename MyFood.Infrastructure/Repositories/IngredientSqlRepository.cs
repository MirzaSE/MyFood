using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Entities;
using MyFood.Application.Services;
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
            return _foodDbContext.Ingredients.FirstOrDefault(x => x.Id == id);
        }

        public void Add(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Add(item);
        }

        public void Delete(int id)
        {
            IngredientEntity ingredientItem = GetSingle(id);

            if (ingredientItem != null)
            {
                _foodDbContext.Ingredients.Remove(ingredientItem);
            }
        }

        public IngredientEntity Update(int id, IngredientEntity item)
        {
            _foodDbContext.Ingredients.Update(item);
            return item;
        }

        public IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<IngredientEntity> _allItems = _foodDbContext.Ingredients.OrderBy(x => x.Name);

            if (queryParameters.HasQuery())
            {
                _allItems = _allItems
                    .Where(x => x.Quantity.ToString().Contains(queryParameters.Query.ToLowerInvariant())
                    || (x.Name != null && x.Name.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant())));
            }

            return _allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public IEnumerable<IngredientEntity> SearchIngredientsByName(string name)
        {
            return _foodDbContext.Ingredients
                .Where(x => x.Name != null && EF.Functions.Like(x.Name, $"%{name}%"))
                .ToList();
        } 

        public int Count()
        {
            return _foodDbContext.Ingredients.Count();
        }

        public bool Save()
        {
            return _foodDbContext.SaveChanges()>=0;
        }
    }
}