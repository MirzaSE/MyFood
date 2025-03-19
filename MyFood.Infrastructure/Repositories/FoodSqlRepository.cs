using MyFood.Application;
﻿using MyFood.Application;
using MyFood.Application.Entities;
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
            return _foodDbContext.FoodItems.FirstOrDefault(x => x.Id == id)!;
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
            IQueryable<FoodEntity> _allItems = _foodDbContext.FoodItems.OrderBy(x=>x.Name);

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

        public void Save()
        {
            _foodDbContext.SaveChanges();
        }

        public ICollection<FoodEntity> GetRandomMeal()
        {
            List<FoodEntity> toReturn = new List<FoodEntity>();

            toReturn.Add(GetRandomItem("Starter"));
            toReturn.Add(GetRandomItem("Main"));
            toReturn.Add(GetRandomItem("Dessert"));

            return toReturn;
        }

        private FoodEntity GetRandomItem(string type)
        {
            var item = _foodDbContext.FoodItems
                .Where(x => x.Type == type)
                .OrderBy(o => Guid.NewGuid())
                .FirstOrDefault();

            if (item == null)
            {
                throw new InvalidOperationException($"No food item found for type {type}");
            }

            return item;
        }

        bool IFoodRepository.Save()
        {
            throw new NotImplementedException();
        }


        public IQueryable<FoodEntity> SearchFoodsByName(string name)
    {
        return _foodDbContext.FoodItems
            .Where(food => food.Name.Contains(name)); // Or use another search logic
    }
    }
}
