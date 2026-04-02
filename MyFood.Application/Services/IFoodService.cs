using MyFood.Application.Dtos;

namespace MyFood.Application.Services;

public interface IFoodService
{
    Task<IEnumerable<FoodDto>> GetAllFoodsAsync(QueryParameters queryParameters);
    Task<int> GetTotalFoodCountAsync();
    Task<FoodDto?> GetFoodByIdAsync(int id);
    Task<IEnumerable<FoodDto>> SearchFoodsByNameAsync(string name);
    Task<FoodDto> CreateFoodAsync(FoodCreateDto foodCreateDto);
    Task<FoodUpdateDto?> GetFoodForUpdateAsync(int id);
    Task<FoodDto?> UpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto);
    Task<bool> DeleteFoodAsync(int id);
    Task<IEnumerable<FoodDto>> GetRandomMealAsync();
}
