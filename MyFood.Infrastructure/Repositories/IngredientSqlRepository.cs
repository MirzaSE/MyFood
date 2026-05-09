using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Services;
using MyFood.Infrastructure.Helpers;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _db;

        public IngredientSqlRepository(FoodDbContext db)
        {
            _db = db;
        }

        public void Add(IngredientEntity item)
        {
            _db.Ingredients.Add(item);
        }

        public void Delete(int id)
        {
            var e = GetSingle(id);
            if (e != null) _db.Ingredients.Remove(e);
        }

        public IngredientEntity GetSingle(int id)
        {
            return _db.Ingredients.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<IngredientEntity> q = _db.Ingredients.OrderBy(i => i.Name);

            if (queryParameters.HasQuery())
            {
                var term = queryParameters.Query.ToLowerInvariant();
                q = q.Where(i => i.Name.ToLowerInvariant().Contains(term));
            }

            return q.Skip(queryParameters.PageCount * (queryParameters.Page - 1)).Take(queryParameters.PageCount);
        }

        public IEnumerable<IngredientEntity> SearchByName(string name)
        {
            return _db.Ingredients.Where(i => EF.Functions.Like(i.Name, $"%{name}%"));
        }

        public IngredientEntity Update(int id, IngredientEntity item)
        {
            _db.Ingredients.Update(item);
            return item;
        }

        public int Count()
        {
            return _db.Ingredients.Count();
        }

        public bool Save()
        {
            return (_db.SaveChanges() >= 0);
        }
    }
}
