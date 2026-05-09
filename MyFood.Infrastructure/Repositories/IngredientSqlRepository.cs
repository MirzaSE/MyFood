using Microsoft.EntityFrameworkCore;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : MyFood.Application.Services.IIngredientRepository
    {
        private readonly FoodDbContext _foodDbContext;

        public IngredientSqlRepository(FoodDbContext foodDbContext)
        {
            _foodDbContext = foodDbContext;
        }

        public IQueryable<IngredientEntity> GetAll()
        {
            return _foodDbContext.Ingredients.OrderBy(i => i.Name);
        }

        public IQueryable<IngredientEntity> SearchByName(string name)
        {
            var normalizedName = name.Trim().ToLowerInvariant();
            return _foodDbContext.Ingredients
                .Where(i => i.Name.ToLower().Contains(normalizedName))
                .OrderBy(i => i.Name);
        }

        public IngredientEntity? GetSingle(int id)
        {
            return _foodDbContext.Ingredients.FirstOrDefault(i => i.Id == id);
        }

        public IngredientEntity? GetByExactName(string name)
        {
            var normalizedName = name.Trim().ToLowerInvariant();
            return _foodDbContext.Ingredients.FirstOrDefault(i => i.Name.ToLower() == normalizedName);
        }

        public void Add(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Add(item);
        }

        public IngredientEntity Update(int id, IngredientEntity item)
        {
            _foodDbContext.Ingredients.Update(item);
            return item;
        }

        public void Delete(int id)
        {
            var ingredient = GetSingle(id);

            if (ingredient != null)
            {
                _foodDbContext.Ingredients.Remove(ingredient);
            }
        }

        public int Count()
        {
            return _foodDbContext.Ingredients.Count();
        }

        public bool Save()
        {
            return _foodDbContext.SaveChanges() >= 0;
        }
    }
}
