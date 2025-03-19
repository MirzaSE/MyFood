using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _context;

        public IngredientSqlRepository(FoodDbContext context)
        {
            _context = context;
        }

        public IngredientEntity GetSingle(int id)
        {
            return _context.Ingredients.FirstOrDefault(i => i.Id == id);
        }

        public void Add(IngredientEntity ingredient)
        {
            _context.Ingredients.Add(ingredient);
        }

        public void Delete(int id)
        {
            var ingredient = GetSingle(id);
            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
            }
        }

        public IngredientEntity Update(int id, IngredientEntity ingredient)
        {
            var existing = GetSingle(id);
            if (existing != null)
            {
                existing.Name = ingredient.Name;
                existing.Quantity = ingredient.Quantity;
                _context.Ingredients.Update(existing);
                return existing;
            }
            return null;
        }

        public IQueryable<IngredientEntity> GetAll()
        {
            return _context.Ingredients;
        }

        public int Count()
        {
            return _context.Ingredients.Count();
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
