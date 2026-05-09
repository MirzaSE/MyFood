using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Services;
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

        public IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters)
        {
            queryParameters ??= new QueryParameters();
            IQueryable<IngredientEntity> allItems = _foodDbContext.Ingredients.OrderBy(x => x.Name);
            if (queryParameters.HasQuery())
            {
                var q = queryParameters.Query.ToLowerInvariant();
                allItems = allItems.Where(x =>
                    (x.Name ?? string.Empty).ToLower().Contains(q) ||
                    (x.Unit ?? string.Empty).ToLower().Contains(q));
            }

            return allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public IEnumerable<IngredientEntity> Search(string query)
        {
            var q = query.ToLowerInvariant();
            return _foodDbContext.Ingredients
                .Where(x =>
                    (x.Name ?? string.Empty).ToLower().Contains(q) ||
                    (x.Unit ?? string.Empty).ToLower().Contains(q))
                .OrderBy(x => x.Name)
                .ToList();
        }

        public IngredientEntity? GetById(int id)
        {
            return _foodDbContext.Ingredients.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<IngredientEntity> GetAllForFood(int foodId)
        {
            return _foodDbContext.Ingredients
                .Where(x => x.FoodEntityId == foodId)
                .OrderBy(x => x.Name)
                .AsNoTracking()
                .ToList();
        }

        public IngredientEntity? GetSingle(int foodId, int ingredientId)
        {
            return _foodDbContext.Ingredients
                .FirstOrDefault(x => x.FoodEntityId == foodId && x.Id == ingredientId);
        }

        public void Add(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Add(item);
        }

        public void Update(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Update(item);
        }

        public void Delete(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Remove(item);
        }

        public bool Save()
        {
            return _foodDbContext.SaveChanges() >= 0;
        }
    }
}
