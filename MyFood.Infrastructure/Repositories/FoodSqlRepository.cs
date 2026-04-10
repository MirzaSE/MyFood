
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

        public async Task<FoodEntity?> GetSingleAsync(int id)
        {
            return await _foodDbContext.FoodItems.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(FoodEntity item)
        {
            await _foodDbContext.FoodItems.AddAsync(item);
        }

        public void Delete(FoodEntity foodItem)
        {
            _foodDbContext.FoodItems.Remove(foodItem);
        }

        public void Update(FoodEntity item)
        {
            _foodDbContext.FoodItems.Update(item);
        }

        public async Task<List<FoodEntity>> GetAllAsync(QueryParameters queryParameters)
        {
            IQueryable<FoodEntity> _allItems = _foodDbContext.FoodItems.OrderBy(x=>x.Name);

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

        public Task<int> CountAsync()
        {
            return _foodDbContext.FoodItems.CountAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return (await _foodDbContext.SaveChangesAsync() >= 0);
        }

        public async Task<List<FoodEntity>> GetRandomMealAsync()
        {
            var meal = new List<FoodEntity?>();
            meal.Add(await GetRandomItemAsync("Starter"));
            meal.Add(await GetRandomItemAsync("Main"));
            meal.Add(await GetRandomItemAsync("Dessert"));
            return meal.Where(x => x is not null).Cast<FoodEntity>().ToList();
        }


        public async Task<List<FoodEntity>> SearchFoodsByNameAsync(string name)
        {
            return await _foodDbContext.FoodItems
                .Where(f => EF.Functions.Like(f.Name, $"%{name}%"))
                .ToListAsync();

            // SELECT * FROM FoodItems WHERE Name LIKE '%name%'
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
