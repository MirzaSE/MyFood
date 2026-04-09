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

        public void AddIngredient(IngredientEntity ingredient)
        {
            _context.Ingredients.Add(ingredient);
            _context.SaveChanges();
        }

        public IEnumerable<IngredientEntity> GetAllIngredients()
        {
            return _context.Ingredients.ToList();
        }

        public IngredientEntity? GetIngredientById(int id)
        {
            return _context.Ingredients.FirstOrDefault(i => i.Id == id);
        }

        public void UpdateIngredient(IngredientEntity ingredient)
        {
            var existing = _context.Ingredients.Find(ingredient.Id);
            if (existing != null)
            {
                existing.Name = ingredient.Name;
                existing.FoodEntityId = ingredient.FoodEntityId;
                _context.SaveChanges(); 
            }
        }

        public void DeleteIngredient(int id)
        {
            var ingredient = _context.Ingredients.Find(id);
            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
                _context.SaveChanges();
            }
        }
    }
}