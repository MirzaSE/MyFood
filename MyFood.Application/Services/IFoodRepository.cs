using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IFoodRepository
    {
        Task<FoodEntity?> GetSingleAsync(int id);
        Task AddAsync(FoodEntity item);
        void Delete(FoodEntity item);
        void Update(FoodEntity item);
        Task<List<FoodEntity>> GetAllAsync(QueryParameters queryParameters);
        Task<List<FoodEntity>> GetRandomMealAsync();
        Task<List<FoodEntity>> SearchFoodsByNameAsync(string name);
        Task<int> CountAsync();
        Task<bool> SaveAsync();
    }
}
