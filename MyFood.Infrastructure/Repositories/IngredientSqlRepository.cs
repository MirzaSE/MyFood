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
            return _foodDbContext.Ingredients.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<IngredientEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<IngredientEntity> items = _foodDbContext.Ingredients.OrderBy(x => x.Name);

            if (!string.IsNullOrWhiteSpace(queryParameters.Query))
            {
                var q = queryParameters.Query.ToLowerInvariant();
                items = items.Where(x => x.Name.ToLower().Contains(q));
            }

            return items
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount)
                .ToList();
        }

        public IEnumerable<IngredientEntity> SearchByName(string name)
        {
            return _foodDbContext.Ingredients
                .Where(i => EF.Functions.Like(i.Name, $"%{name}%"))
                .ToList();
        }

        public void Add(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Add(item);
        }

        public void Delete(int id)
        {
            var existing = GetSingle(id);
            if (existing != null)
            {
                _foodDbContext.Ingredients.Remove(existing);
            }
        }

        public IngredientEntity Update(int id, IngredientEntity item)
        {
            _foodDbContext.Ingredients.Update(item);
            return item;
        }

        public int Count()
        {
            return _foodDbContext.Ingredients.Count();
        }

        public bool Save()
        {
            return _foodDbContext.SaveChanges() >= 0;
        }
    }
}
