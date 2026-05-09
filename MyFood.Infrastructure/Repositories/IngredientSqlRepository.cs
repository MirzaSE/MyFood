using MyFood.Application;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using MyFood.Infrastructure.Helpers;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _context;

        public IngredientSqlRepository(FoodDbContext context)
        {
            _context = context;
        }

        public IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<IngredientEntity> query = _context.Ingredients.OrderBy(i => i.Name);

            if (queryParameters.HasQuery())
            {
                var needle = queryParameters.Query!.ToLower();
                query = query.Where(i => i.Name.ToLower().Contains(needle));
            }

            return query
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public IngredientEntity? GetSingle(int id)
        {
            return _context.Ingredients.FirstOrDefault(i => i.Id == id);
        }

        public IEnumerable<IngredientEntity> SearchByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return _context.Ingredients.ToList();
            }

            // SQL Server collation is case-insensitive by default. ToLower keeps
            // the intent explicit and works for in-memory providers too.
            var needle = name.ToLower();
            return _context.Ingredients
                .Where(i => i.Name.ToLower().Contains(needle))
                .ToList();
        }

        public bool ExistsByName(string name, int? excludingId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            var needle = name.ToLower();
            var query = _context.Ingredients.Where(i => i.Name.ToLower() == needle);

            if (excludingId.HasValue)
            {
                query = query.Where(i => i.Id != excludingId.Value);
            }

            return query.Any();
        }

        public void Add(IngredientEntity item)
        {
            _context.Ingredients.Add(item);
        }

        public IngredientEntity Update(int id, IngredientEntity item)
        {
            // Caller has already loaded a tracked entity; EF picks up the changes.
            return item;
        }

        public void Delete(int id)
        {
            var entity = _context.Ingredients.FirstOrDefault(i => i.Id == id);
            if (entity != null)
            {
                _context.Ingredients.Remove(entity);
            }
        }

        public int Count()
        {
            return _context.Ingredients.Count();
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}
