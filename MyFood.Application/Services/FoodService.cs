using MyFood.Application;
using MyFood.Application.Entities;
using MyFood.Application.Repositories;

namespace MyFood.Application.Services
{
    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _foodRepository;

        public FoodService(IFoodRepository foodRepository)
        {
            _foodRepository = foodRepository;
        }

        public IQueryable<FoodEntity> GetAll(QueryParameters queryParameters)
        {
            return _foodRepository.GetAll(queryParameters);
        }

        public FoodEntity? GetSingle(int id)
        {
            return _foodRepository.GetSingle(id);
        }

        public IEnumerable<FoodEntity> SearchFoodsByName(string name)
        {
            return _foodRepository.SearchFoodsByName(name);
        }

        public FoodEntity Add(FoodEntity item)
        {
            _foodRepository.Add(item);

            if (!_foodRepository.Save())
            {
                throw new Exception("Creating a food item failed on save.");
            }

            return item;
        }

        public FoodEntity Update(int id, FoodEntity item)
        {
            var existing = _foodRepository.GetSingle(id);

            if (existing == null)
            {
                throw new Exception("Food item not found.");
            }

            item.Id = id;

            var updated = _foodRepository.Update(id, item);

            if (!_foodRepository.Save())
            {
                throw new Exception("Updating a food item failed on save.");
            }

            return updated;
        }

        public void Delete(int id)
        {
            var existing = _foodRepository.GetSingle(id);

            if (existing == null)
            {
                throw new Exception("Food item not found.");
            }

            _foodRepository.Delete(id);

            if (!_foodRepository.Save())
            {
                throw new Exception("Deleting a food item failed on save.");
            }
        }

        public ICollection<FoodEntity> GetRandomMeal()
        {
            return _foodRepository.GetRandomMeal();
        }

        public int Count()
        {
            return _foodRepository.Count();
        }
    }
}