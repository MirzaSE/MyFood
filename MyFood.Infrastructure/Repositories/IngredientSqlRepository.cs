using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _context;

        public IngredientSqlRepository(FoodDbContext context)
        {
            _context = context;
        }

        public IQueryable<IngredientEntity> GetAll()
        {
            return _context.Ingredients.OrderBy(i => i.Name);
        }

        public IngredientEntity? GetSingle(int id)
        {
            return _context.Ingredients.FirstOrDefault(i => i.Id == id);
        }

        public void Add(IngredientEntity ingredient)
        {
            _context.Ingredients.Add(ingredient);
        }

        public IngredientEntity Update(int id, IngredientEntity ingredient)
        {
            _context.Ingredients.Update(ingredient);
            return ingredient;
        }

        public void Delete(int id)
        {
            var ingredient = GetSingle(id);

            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
            }
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}