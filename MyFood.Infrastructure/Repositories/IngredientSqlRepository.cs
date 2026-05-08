using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _foodDbContext;

        public IngredientSqlRepository(FoodDbContext foodDbContext)
        {
            _foodDbContext = foodDbContext;
        }

        public IngredientEntity? GetSingle(int id)
        {
            return _foodDbContext.Ingredients.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<IngredientEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<IngredientEntity> allItems = _foodDbContext.Ingredients.OrderBy(x => x.Name);

            if (!string.IsNullOrWhiteSpace(queryParameters.Query))
            {
                var lowered = queryParameters.Query.ToLowerInvariant();
                allItems = allItems.Where(x => x.Name.ToLower().Contains(lowered));
            }

            return allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount)
                .ToList();
        }

        public IEnumerable<IngredientEntity> GetByFoodId(int foodId)
        {
            return _foodDbContext.FoodIngredients
                .Where(x => x.FoodEntityId == foodId)
                .Include(x => x.IngredientEntity)
                .Select(x => x.IngredientEntity!)
                .ToList();
        }

        public IEnumerable<IngredientEntity> SearchByName(string name)
        {
            return _foodDbContext.Ingredients
                .Where(x => EF.Functions.Like(x.Name, $"%{name}%"))
                .ToList();
        }

        public void Add(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Add(item);
        }

        public void Delete(int id)
        {
            IngredientEntity? ingredient = GetSingle(id);
            if (ingredient != null)
            {
                _foodDbContext.Ingredients.Remove(ingredient);
            }
        }

        public IngredientEntity Update(int id, IngredientEntity item)
        {
            _foodDbContext.Ingredients.Update(item);
            return item;
        }

        public int Count()
        {
            return _foodDbContext.Ingredients.Count();
        }

        public bool Save()
        {
            return (_foodDbContext.SaveChanges() >= 0);
        }
    }
}