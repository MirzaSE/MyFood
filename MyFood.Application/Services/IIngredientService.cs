using MyFood.Application.Dtos;

namespace MyFood.Application.Services
{
    public interface IIngredientService
    {
        Task<IEnumerable<IngredientDto>> GetAllAsync(QueryParameters queryParameters);
        Task<IngredientDto?> GetByIdAsync(int id);
        Task<IEnumerable<IngredientDto>> SearchAsync(string name);
        Task<IngredientDto> CreateAsync(IngredientCreateDto ingredientCreateDto);
        Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto ingredientUpdateDto);
        Task<bool> DeleteAsync(int id);
    }
}
