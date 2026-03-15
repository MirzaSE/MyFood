using MyFood.Application.Entities;
using MyFood.Infrastructure.Repositories;

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
            _context.Ingredients.Update(ingredient);
        }
        public void DeleteIngredient(int id)
        {
            var ingredient = _context.Ingredients.Find(id);
            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
            }
        }
        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}