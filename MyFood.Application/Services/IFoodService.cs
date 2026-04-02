using MyFood.Application.Dtos;
using MyFood.Application;
using Microsoft.AspNetCore.JsonPatch;  // Add this for JsonPatchDocument

namespace MyFood.Application.Services
{
    public interface IFoodService
    {
        Task<IEnumerable<FoodDto>> GetAllFoodsAsync(QueryParameters queryParameters);
        Task<FoodDto?> GetFoodByIdAsync(int id);
        Task<(IEnumerable<FoodDto> foodDtos, int totalCount)> SearchFoodsByNameAsync(string name, QueryParameters queryParameters);  // Change to return tuple
        Task<FoodDto> AddFoodAsync(FoodCreateDto foodCreateDto);  // Change from CreateFoodAsync to AddFoodAsync
        Task<FoodDto?> UpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto);
        Task<FoodDto?> PartiallyUpdateFoodAsync(int id, JsonPatchDocument<FoodUpdateDto> patchDoc);  // Add this method
        Task<bool> DeleteFoodAsync(int id);
        Task<IEnumerable<FoodDto>> GetRandomMealAsync();
        Task<int> GetTotalFoodCountAsync();
    }
}
