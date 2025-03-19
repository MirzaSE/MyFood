using MyFood.Application.Entities;
using MyFood.Application.Interfaces;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _context;

        public IngredientSqlRepository(FoodDbContext context)
        {
            _context = context;
        }
        public void Add(IngredientEntity ingredient)
        {
            _context.Ingredients.Add(ingredient);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var ingridient = _context.Ingredients.Find(id);
            if (ingridient != null) 
            {
                _context.Remove(ingridient);
                _context.SaveChanges();
            }
        }

        public List<IngredientEntity> GetAll()
        {
            return _context.Ingredients.ToList();
        }

        public IngredientEntity? GetById(int id)
        {
            return _context.Ingredients.Find(id);
        }

        public void Update(IngredientEntity ingredient)
        {
            _context.Ingredients.Update(ingredient);
            _context.SaveChanges();
        }
    }
}
        
       