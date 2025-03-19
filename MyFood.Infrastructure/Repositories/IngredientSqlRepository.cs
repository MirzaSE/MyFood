using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Entities;
using MyFood.Application.Repositories;
using MyFood.Infrastructure.Helpers;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _foodDbContext;
        
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

        
        public IngredientSqlRepository(FoodDbContext foodDbContext)
        {
            _foodDbContext = foodDbContext;
        }
        public void Add(IngredientEntity ingredient)
        {
            _foodDbContext.Ingredients.Add(ingredient);
        }
        public IngredientEntity GetSingle(int id)
        {
            return _foodDbContext.Ingredients.FirstOrDefault(i => i.Id == id);
        }
        
        public IEnumerable<IngredientEntity> GetAll()
        {
            return _foodDbContext.Ingredients.ToList();
        }
 
        public void Update(IngredientEntity ingredient)
        {
            _foodDbContext.Ingredients.Update(ingredient);
        }
        
        public void Delete(int id)
        {
            var ingredient = GetSingle(id);
            if (ingredient != null)
            {
                _foodDbContext.Ingredients.Remove(ingredient);
            }
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