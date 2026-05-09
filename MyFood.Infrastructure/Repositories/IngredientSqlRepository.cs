using MyFood.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Added for EntityState

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _context;

        public IngredientSqlRepository(FoodDbContext context)
        {
            _context = context;
        }

        public ICollection<IngredientEntity> GetAll() => _context.Ingredients.ToList();

        public IngredientEntity GetSingle(int id) => _context.Ingredients.FirstOrDefault(x => x.Id == id);

        public ICollection<IngredientEntity> Search(string name)
            => _context.Ingredients.Where(x => x.Name.Contains(name)).ToList();

        public void Add(IngredientEntity item) => _context.Ingredients.Add(item);

        public void Delete(int id)
        {
            var item = GetSingle(id);
            if (item != null) _context.Ingredients.Remove(item);
        }

        public IngredientEntity Update(IngredientEntity item)
        {
            // Check if the item is already being tracked by the context
            var local = _context.Ingredients
                .Local
                .FirstOrDefault(entry => entry.Id == item.Id);

            // If it is tracking it, "detach" it so we can update with the new version
            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Ingredients.Update(item);
            return item;
        }

        public bool Save() => _context.SaveChanges() >= 0;
    }
}