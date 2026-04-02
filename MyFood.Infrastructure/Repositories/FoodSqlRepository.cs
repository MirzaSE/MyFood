using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Services;
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

        public IQueryable<FoodEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<FoodEntity> _allItems = _foodDbContext.FoodItems.OrderBy(x => x.Name);

            if (queryParameters.HasQuery())
            {
                _allItems = _allItems
                    .Where(x => x.Calories.ToString().Contains(queryParameters.Query.ToLowerInvariant())
                    || x.Name.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant()));
            }

            return _allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
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

        public Task<FoodEntity?> GetSingleAsync(int id)
        {
            return _foodDbContext.FoodItems.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<int> CountAsync()
        {
            return _foodDbContext.FoodItems.CountAsync();
        }

        public async Task<IEnumerable<FoodEntity>> GetAllAsync(QueryParameters queryParameters)
        {
            IQueryable<FoodEntity> _allItems = _foodDbContext.FoodItems.OrderBy(x => x.Name);

            if (queryParameters.HasQuery())
            {
                _allItems = _allItems
                    .Where(x => x.Calories.ToString().Contains(queryParameters.Query.ToLowerInvariant())
                    || x.Name.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant()));
            }

            return await _allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount)
                .ToListAsync();
        }

        public async Task<IEnumerable<FoodEntity>> SearchFoodsByNameAsync(string name)
        {
            return await _foodDbContext.FoodItems
                .Where(f => EF.Functions.Like(f.Name, $"%{name}%"))
                .ToListAsync();
        }

        public async Task<ICollection<FoodEntity>> GetRandomMealAsync()
        {
            var toReturn = new List<FoodEntity>();

            toReturn.Add(await GetRandomItemAsync("Starter"));
            toReturn.Add(await GetRandomItemAsync("Main"));
            toReturn.Add(await GetRandomItemAsync("Dessert"));

            return toReturn;
        }

        public async Task AddAsync(FoodEntity item)
        {
            await _foodDbContext.FoodItems.AddAsync(item);
        }

        public async Task DeleteAsync(int id)
        {
            var foodItem = await GetSingleAsync(id);
            if (foodItem != null)
            {
                _foodDbContext.FoodItems.Remove(foodItem);
            }
        }

        public async Task<FoodEntity?> UpdateAsync(int id, FoodEntity item)
        {
            _foodDbContext.FoodItems.Update(item);
            return item;
        }

        public async Task<bool> SaveAsync()
        {
            return (await _foodDbContext.SaveChangesAsync() >= 0);
        }

        private async Task<FoodEntity?> GetRandomItemAsync(string type)
        {
            return await _foodDbContext.FoodItems
                .Where(x => x.Type == type)
                .OrderBy(o => Guid.NewGuid())
                .FirstOrDefaultAsync();
        }
    }
}
