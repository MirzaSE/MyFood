using MyFood.Application.Dtos;

namespace MyFood.Application.Services;

public interface IIngredientService
{
    Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync(QueryParameters queryParameters);
    Task<IngredientDto?> GetIngredientByIdAsync(int id);
    Task<IEnumerable<IngredientDto>> SearchIngredientsByNameAsync(string name);
    Task<IngredientDto> CreateIngredientAsync(IngredientCreateDto ingredientCreateDto);
    Task<IngredientDto?> UpdateIngredientAsync(int id, IngredientUpdateDto ingredientUpdateDto);
    Task<bool> DeleteIngredientAsync(int id);
    Task<int> GetTotalIngredientCountAsync();
}