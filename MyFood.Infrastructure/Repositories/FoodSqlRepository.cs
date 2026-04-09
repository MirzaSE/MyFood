

using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Domain.Entities;
using MyFood.Infrastructure.Helpers;

namespace MyFood.Infrastructure.Repositories
{
    public class FoodSqlRepository : IFoodRepository
    {
        private readonly FoodDbContext _foodDbContext;

        public FoodSqlRepository(FoodDbContext foodDbContext)
        {
            _foodDbContext = foodDbContext;
        }

        public FoodEntity? GetSingle(int id)
        {
            return _foodDbContext.FoodItems.FirstOrDefault(x => x.Id == id);
        }

        public void Add(FoodEntity item)
        {
            _foodDbContext.FoodItems.Add(item);
        }

        public void Delete(int id)
        {
            var foodItem = GetSingle(id);
            if (foodItem is not null)
            {
                _foodDbContext.FoodItems.Remove(foodItem);
            }
        }

        public FoodEntity Update(int id, FoodEntity item)
        {
            _foodDbContext.FoodItems.Update(item);
            return item;
        }

        public IQueryable<FoodEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<FoodEntity> _allItems = _foodDbContext.FoodItems.OrderBy(x => x.Name);

            if (queryParameters.HasQuery() && !string.IsNullOrWhiteSpace(queryParameters.Query))
            {
                var query = queryParameters.Query.ToLowerInvariant();
                _allItems = _allItems.Where(x =>
                    x.Calories.ToString().Contains(query) ||
                    (x.Name != null && x.Name.ToLowerInvariant().Contains(query))
                );
            }

            return _allItems;
        }
        public int Count()
        {
            return _foodDbContext.FoodItems.Count();
        }

        public bool Save()
        {
            return (_foodDbContext.SaveChanges() >= 0);
        }

        public ICollection<FoodEntity> GetRandomMeal()
        {
            List<FoodEntity> toReturn = new List<FoodEntity>();

            var starter = GetRandomItem("Starter");
            if (starter != null) toReturn.Add(starter);
            var main = GetRandomItem("Main");
            if (main != null) toReturn.Add(main);
            var dessert = GetRandomItem("Dessert");
            if (dessert != null) toReturn.Add(dessert);

            return toReturn;
        }


        public IEnumerable<FoodEntity> SearchFoodsByName(string name)
        {
            return _foodDbContext.FoodItems
                .Where(f => EF.Functions.Like(f.Name, $"%{name}%"))
                .ToList();

            // SELECT * FROM FoodItems WHERE Name LIKE '%name%'
        }

        private FoodEntity? GetRandomItem(string type)
        {
            return _foodDbContext.FoodItems
                .Where(x => x.Type == type)
                .OrderBy(o => Guid.NewGuid())
                .FirstOrDefault();
        }
    }
}
