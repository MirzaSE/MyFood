using MyFood.Application;
using MyFood.Application.Dtos;

namespace MyFood.Application.Services
{
    public interface IIngredientService
    {
        Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync(QueryParameters? queryParameters = null);
        Task<IngredientDto?> GetIngredientByIdAsync(int id);
        Task<IEnumerable<IngredientDto>> SearchIngredientsByNameAsync(string name);
        Task<IngredientDto> CreateIngredientAsync(CreateIngredientDto createIngredientDto);
        Task<IngredientDto?> UpdateIngredientAsync(int id, UpdateIngredientDto updateIngredientDto);
        Task<bool> DeleteIngredientAsync(int id);
    }
}