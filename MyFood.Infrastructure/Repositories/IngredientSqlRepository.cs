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

        public IEnumerable<IngredientEntity> GetAll()
        {
            return _context.Ingredients.ToList();
        }

        public IngredientEntity GetSingle(int id)
        {
            return _context.Ingredients.FirstOrDefault(i => i.Id == id);
        }

        public IEnumerable<IngredientEntity> GetByFoodId(int foodId)
        {
            return _context.Ingredients
                .Where(i => i.FoodId == foodId)
                .ToList();
        }

        public void Add(IngredientEntity ingredient)
        {
            _context.Ingredients.Add(ingredient);
        }

        public IngredientEntity Update(int id, IngredientEntity ingredient)
        {
            var existingIngredient = _context.Ingredients.FirstOrDefault(i => i.Id == id);

            if (existingIngredient == null)
            {
                return null;
            }

            existingIngredient.Name = ingredient.Name;
            existingIngredient.Quantity = ingredient.Quantity;
            existingIngredient.FoodId = ingredient.FoodId;

            return existingIngredient;
        }

        public void Delete(int id)
        {
            var ingredient = _context.Ingredients.FirstOrDefault(i => i.Id == id);

            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
            }
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}