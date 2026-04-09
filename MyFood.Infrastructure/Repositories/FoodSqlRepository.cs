using MyFood.Application;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public class FoodSqlRepository : MyFood.Application.Services.IFoodRepository
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
            if (foodItem != null)
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
            var allItems = _foodDbContext.FoodItems.OrderBy(x => x.Name).AsQueryable();

            if (queryParameters.HasQuery())
            {
                var loweredQuery = queryParameters.Query.ToLowerInvariant();
                allItems = allItems.Where(x =>
                    x.Calories.ToString().Contains(loweredQuery) ||
                    (x.Name != null && x.Name.ToLowerInvariant().Contains(loweredQuery)));
            }

            return allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public IQueryable<FoodEntity> SearchFoodsByName(string name)
        {
            var foodItems = _foodDbContext.FoodItems.OrderBy(x => x.Name).AsQueryable();

            if (string.IsNullOrWhiteSpace(name))
            {
                return foodItems;
            }

            var loweredName = name.ToLowerInvariant();
            return foodItems.Where(x => x.Name != null && x.Name.ToLowerInvariant().Contains(loweredName));
        }

        public int Count()
        {
            return _foodDbContext.FoodItems.Count();
        }

        public bool Save()
        {
            return _foodDbContext.SaveChanges() >= 0;
        }

        public ICollection<FoodEntity> GetRandomMeal()
        {
            var toReturn = new List<FoodEntity>();

            var starter = GetRandomItem("Starter");
            var main = GetRandomItem("Main");
            var dessert = GetRandomItem("Dessert");

            if (starter != null) toReturn.Add(starter);
            if (main != null) toReturn.Add(main);
            if (dessert != null) toReturn.Add(dessert);

            return toReturn;
        }

        private FoodEntity? GetRandomItem(string type)
        {
            return _foodDbContext.FoodItems
                .Where(x => x.Type == type)
                .OrderBy(_ => Guid.NewGuid())
                .FirstOrDefault();
        }
    }
}