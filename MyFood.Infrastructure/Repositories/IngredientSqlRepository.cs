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
            return _context.Ingredients.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<IngredientEntity> GetAllForFood(int foodId)
        {
            return _context.Ingredients.Where(x => x.FoodId == foodId).ToList();
        }

        public void Add(IngredientEntity item)
        {
            _context.Ingredients.Add(item);
        }

        public IngredientEntity Update(int id, IngredientEntity item)
        {
            _context.Ingredients.Update(item);
            return item;
        }

        public void Delete(int id)
        {
            IngredientEntity ingredient = GetSingle(id);
            _context.Ingredients.Remove(ingredient);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
