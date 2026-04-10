using Microsoft.AspNetCore.JsonPatch;
using MyFood.Application;
using MyFood.Application.Dtos;

public interface IFoodService
{
    Task<IEnumerable<FoodDto>> GetAllFoodsAsync(QueryParameters queryParameters);
    Task<FoodDto?> GetFoodByIdAsync(int id);

    Task<IEnumerable<FoodDto>> SearchFoodsByNameAsync(string name);
    Task<FoodDto> CreateFoodAsync(FoodCreateDto foodCreateDto);
    Task<FoodDto?> UpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto);
    Task<FoodDto?> PartialUpdateFoodAsync(int id, JsonPatchDocument<FoodUpdateDto> patchDoc);
    Task<bool> DeleteFoodAsync(int id);
    Task<IEnumerable<FoodDto>> GetRandomMealAsync();
    Task<int> GetTotalFoodCountAsync();

}
