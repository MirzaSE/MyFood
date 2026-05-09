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
            var item = GetSingle(id);
            if (item != null)
            {
                _foodDbContext.Ingredients.Remove(item);
            }
        }

        public IngredientEntity Update(int id, IngredientEntity item)
        {
            _foodDbContext.Ingredients.Update(item);
            return item;
        }

        public IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<IngredientEntity> all = _foodDbContext.Ingredients.OrderBy(x => x.Name);

            if (queryParameters.HasQuery())
            {
                var q = queryParameters.Query.ToLowerInvariant();
                all = all.Where(x => x.Name.ToLower().Contains(q) || x.Unit.ToLower().Contains(q));
            }

            return all
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public IEnumerable<IngredientEntity> SearchByName(string name)
        {
            var pattern = $"%{name}%";
            return _foodDbContext.Ingredients
                .Where(i => EF.Functions.Like(i.Name, pattern))
                .ToList();
        }

        public bool ExistsByName(string name)
        {
            var lowered = (name ?? string.Empty).ToLower();
            return _foodDbContext.Ingredients.Any(i => i.Name.ToLower() == lowered);
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
