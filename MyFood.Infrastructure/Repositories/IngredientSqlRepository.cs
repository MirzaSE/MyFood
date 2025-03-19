using MyFood.Application.Entities;
using MyFood.Application;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void Add(IngredientEntity ingredient)
        {
            _foodDbContext.Ingredients.Add(ingredient);
        }

        public void Delete(int id)
        {
            IngredientEntity ingredient = GetSingle(id);
            if (ingredient != null)
            {
                _foodDbContext.Ingredients.Remove(ingredient);
            }
        }

        public IngredientEntity Update(int id, IngredientEntity ingredient)
        {
            _foodDbContext.Ingredients.Update(ingredient);
            return ingredient;
        }

        public IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<IngredientEntity> _allItems = _foodDbContext.Ingredients.OrderBy(x => x.Name);

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
            return _foodDbContext.FoodItems.Count();
        }

        public bool Save()
        {
            return (_foodDbContext.SaveChanges() >= 0);
        }

       

        
    }
}

