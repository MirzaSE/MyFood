using Microsoft.EntityFrameworkCore;
using MyFood.Domain.Entities;
using MyFood.Domain.Interfaces;

namespace MyFood.Infrastructure.Repositories
{
    public class FoodSqlRepository : IFoodRepository
    {
        private readonly FoodDbContext _foodDbContext;

        public FoodSqlRepository(FoodDbContext foodDbContext)
        {
            _foodDbContext = foodDbContext;
        }

        public FoodEntity GetSingle(int id)
        {
            return _foodDbContext.FoodItems.FirstOrDefault(x => x.Id == id);
        }

        public void Add(FoodEntity item)
        {
            _foodDbContext.FoodItems.Add(item);
        }

        public void Delete(int id)
        {
            FoodEntity foodItem = GetSingle(id);
            _foodDbContext.FoodItems.Remove(foodItem);
        }

        public FoodEntity Update(int id, FoodEntity item)
        {
            _foodDbContext.FoodItems.Update(item);
            return item;
        }

        public IQueryable<FoodEntity> GetAll(string query, int pageCount, int page)
        {
            IQueryable<FoodEntity> _allItems = _foodDbContext.FoodItems.OrderBy(x=>x.Name);

            if (!string.IsNullOrWhiteSpace(query))
            {
                _allItems = _allItems
                    .Where(x => x.Calories.ToString().Contains(query.ToLowerInvariant())
                    || x.Name.ToLowerInvariant().Contains(query.ToLowerInvariant()));
            }

            return _allItems
                .Skip(pageCount * (page - 1))
                .Take(pageCount);
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

            toReturn.Add(GetRandomItem("Starter"));
            toReturn.Add(GetRandomItem("Main"));
            toReturn.Add(GetRandomItem("Dessert"));

            return toReturn;
        }


        public IEnumerable<FoodEntity> SearchFoodsByName(string name)
        {
            return _foodDbContext.FoodItems
                .Where(f => EF.Functions.Like(f.Name, $"%{name}%"))
                .ToList();

            // SELECT * FROM FoodItems WHERE Name LIKE '%name%'
        }

        private FoodEntity GetRandomItem(string type)
        {
            return _foodDbContext.FoodItems
                .Where(x => x.Type == type)
                .OrderBy(o => Guid.NewGuid())
                .FirstOrDefault();
        }
    }
}
