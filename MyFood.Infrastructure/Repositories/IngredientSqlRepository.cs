using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Domain.Entities;
using MyFood.Infrastructure.Helpers;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository: IIngredientRepository
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
            IngredientEntity ingredient = GetSingle(id);
            _foodDbContext.Ingredients.Remove(ingredient);
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
                    .Where(x => x.Name.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant()));
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

        public IEnumerable<IngredientEntity> SearchIngredientByName(string name)
        {
            return _foodDbContext.Ingredients
                .Where(x => EF.Functions.Like(x.Name, $"%{name}%"))
                .ToList();
        }
    }
}
