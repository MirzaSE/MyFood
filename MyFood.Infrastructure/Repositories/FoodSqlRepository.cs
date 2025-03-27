

using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using Microsoft.EntityFrameworkCore;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFood.Infrastructure.Repositories
{
    public class FoodSqlRepository : IFoodRepository
    {
        private readonly FoodDbContext _foodDbContext;
        private readonly ILogger<FoodSqlRepository> _logger;

        public FoodSqlRepository(FoodDbContext foodDbContext, ILogger<FoodSqlRepository> logger)
        {
            _foodDbContext = foodDbContext;
            _logger = logger;
        }

        // Async version with exception handling
        public async Task<FoodEntity> GetSingleAsync(int id)
        {
            if (id < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "The ID must be greater than or equal to 1.");
            }

            try
            {
                var foodItem = await _foodDbContext.FoodItems
                                                   .FirstOrDefaultAsync(x => x.Id == id);

                if (foodItem == null)
                {
                    throw new KeyNotFoundException("Food not found.");
                }

                return foodItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching food item with ID {Id}", id);
                throw;
            }
        }

        // Synchronous version of GetSingle with exception handling
        public FoodEntity GetSingle(int id)
        {
            if (id < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "The ID must be greater than or equal to 1.");
            }

            var foodItem = _foodDbContext.FoodItems.FirstOrDefault(x => x.Id == id);
            if (foodItem == null)
            {
                throw new KeyNotFoundException("Food not found.");
            }

            return foodItem;
        }

        public void Add(FoodEntity item)
        {
            _foodDbContext.FoodItems.Add(item);
        }

        public void Delete(int id)
        {
            var foodItem = GetSingle(id);
            _foodDbContext.FoodItems.Remove(foodItem);
        }

        public FoodEntity Update(int id, FoodEntity item)
        {
            var existingItem = GetSingle(id);
            // Optionally check if the item being updated matches any other necessary validation logic
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
            try
            {
                return (_foodDbContext.SaveChanges() >= 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving changes to the database.");
                return false;
            }
        }

        public ICollection<FoodEntity> GetRandomMeal()
        {
            try
            {
                List<FoodEntity> toReturn = new List<FoodEntity>
                {
                    GetRandomItem("Starter"),
                    GetRandomItem("Main"),
                    GetRandomItem("Dessert")
                };

                return toReturn;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching random meal.");
                return new List<FoodEntity>();
            }
        }

        public IEnumerable<FoodEntity> SearchFoodsByName(string name)
        {
            return _foodDbContext.FoodItems
                .Where(f => EF.Functions.Like(f.Name, $"%{name}%"))
                .ToList();
        }

        private FoodEntity GetRandomItem(string type)
        {
            var randomItem = _foodDbContext.FoodItems
                .Where(x => x.Type == type)
                .OrderBy(o => Guid.NewGuid())
                .FirstOrDefault();

            if (randomItem == null)
            {
                throw new KeyNotFoundException($"{type} not found.");
            }

            return randomItem;
        }
    }
}
