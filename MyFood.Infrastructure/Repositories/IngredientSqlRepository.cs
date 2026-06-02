using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Services;
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

        public IngredientEntity? GetSingle(int id)
        {
            return _foodDbContext.Ingredients.FirstOrDefault(x => x.Id == id && x.FoodEntityId == null);
        }

        public void Add(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Add(item);
        }

        public void Delete(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Remove(item);
        }

        public IngredientEntity Update(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Update(item);
            return item;
        }

        public IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<IngredientEntity> query = _foodDbContext.Ingredients
                .Where(x => x.FoodEntityId == null)
                .OrderBy(x => x.Name);

            if (queryParameters.HasQuery())
            {
                string lower = queryParameters.Query!.ToLowerInvariant();
                query = query.Where(x => x.Name!.ToLower().Contains(lower)
                                         || x.Unit!.ToLower().Contains(lower));
            }

            return query
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public bool ExistsByName(string name)
        {
            return _foodDbContext.Ingredients
                .Any(x => x.FoodEntityId == null && x.Name!.ToLower() == name.ToLowerInvariant());
        }

        public IEnumerable<IngredientEntity> SearchByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Enumerable.Empty<IngredientEntity>();
            }

            var lower = name.ToLowerInvariant();
            return _foodDbContext.Ingredients
                .Where(x => x.FoodEntityId == null && x.Name!.ToLower().Contains(lower))
                .OrderBy(x => x.Name)
                .ToList();
        }

        public int Count()
        {
            return _foodDbContext.Ingredients.Count(x => x.FoodEntityId == null);
        }

        public bool Save()
        {
            return (_foodDbContext.SaveChanges() >= 0);
        }
    }
}
