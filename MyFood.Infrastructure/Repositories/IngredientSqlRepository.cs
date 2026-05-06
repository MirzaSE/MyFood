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
            IQueryable<IngredientEntity> allItems = _foodDbContext.Ingredients.OrderBy(x => x.Name);

            if (queryParameters.HasQuery())
            {
                var q = queryParameters.Query!.ToLowerInvariant();
                allItems = allItems.Where(x =>
                    x.Name.ToLower().Contains(q) ||
                    x.Unit.ToLower().Contains(q));
            }

            return allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public IngredientEntity? GetSingle(int id)
        {
            return _foodDbContext.Ingredients.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<IngredientEntity> SearchByName(string name)
        {
            return _foodDbContext.Ingredients
                .Where(i => EF.Functions.Like(i.Name, $"%{name}%"))
                .OrderBy(i => i.Name)
                .ToList();
        }

        public bool ExistsByName(string name, int? excludingId = null)
        {
            var query = _foodDbContext.Ingredients
                .Where(i => i.Name.ToLower() == name.ToLower());

            if (excludingId.HasValue)
            {
                query = query.Where(i => i.Id != excludingId.Value);
            }

            return query.Any();
        }

        public int Count(string? query = null)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return _foodDbContext.Ingredients.Count();
            }

            var q = query.ToLowerInvariant();
            return _foodDbContext.Ingredients.Count(i =>
                i.Name.ToLower().Contains(q) || i.Unit.ToLower().Contains(q));
        }

        public void Add(IngredientEntity entity)
        {
            _foodDbContext.Ingredients.Add(entity);
        }

        public IngredientEntity Update(IngredientEntity entity)
        {
            _foodDbContext.Ingredients.Update(entity);
            return entity;
        }

        public void Delete(IngredientEntity entity)
        {
            _foodDbContext.Ingredients.Remove(entity);
        }

        public bool Save()
        {
            return _foodDbContext.SaveChanges() >= 0;
        }
    }
}
