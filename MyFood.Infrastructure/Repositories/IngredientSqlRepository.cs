

using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Entities;
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

        public void Add(IngredientEntity item)
        {
            _foodDbContext.IngredientEntities.Add(item);
        }
        
        public IngredientEntity GetSingle(int id)
        {
            return _foodDbContext.IngredientEntities.FirstOrDefault(i => i.Id == id);
        }

        public void Delete(int id)
        {
            IngredientEntity? ingredientItem = GetSingle(id);
            if(ingredientItem == null)
            {
                return;
            }
            _foodDbContext.IngredientEntities.Remove(ingredientItem);
        }

        public IngredientEntity Update(int id, IngredientEntity item)
        {
            _foodDbContext.IngredientEntities.Update(item);
            return item;
        }

        public IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<IngredientEntity> query = _foodDbContext.IngredientEntities.OrderBy(x => x.Name);

            if (queryParameters.HasQuery())
            {
                query = query.Where(x =>
                    x.Name != null &&
                    x.Name.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant()));
            }

            return query
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public int Count()
        {
            return _foodDbContext.IngredientEntities.Count();
        }
        
        public bool Save()
        {
            return _foodDbContext.SaveChanges() >=0;
        }

        public IEnumerable<IngredientEntity> SearchIngredientsByName(string name)
        {
            return _foodDbContext.IngredientEntities    
            .Where(f => EF.Functions.Like(f.Name ?? string.Empty, $"%{name}%"));
;
        } 
    
    }
}
