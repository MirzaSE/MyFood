using Microsoft.EntityFrameworkCore;
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
            return _context.Ingredients.AsNoTracking();
        }

        public IngredientEntity? GetById(int id)
        {
            return _context.Ingredients.FirstOrDefault(i => i.Id == id);
        }

        public IngredientEntity? GetByName(string name)
        {
            return _context.Ingredients
                .FirstOrDefault(i => i.Name.ToLower() == name.ToLower());
        }

        public void Add(IngredientEntity ingredient)
        {
            _context.Ingredients.Add(ingredient);
        }

        public void Update(IngredientEntity ingredient)
        {
            _context.Ingredients.Update(ingredient);
        }

        public void Delete(IngredientEntity ingredient)
        {
            _context.Ingredients.Remove(ingredient);
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}