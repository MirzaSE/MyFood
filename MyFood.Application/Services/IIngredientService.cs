using MyFood.Application.Dtos;

namespace MyFood.Application.Services
{
    public interface IIngredientService
    {
        Task<IEnumerable<IngredientDto>> GetAllAsync(QueryParameters? parameters = null);
        Task<IngredientDto?> GetByIdAsync(int id);
        Task<IngredientDto> CreateAsync(IngredientCreateDto ingredientCreateDto);
        Task<IngredientDto> UpdateAsync(int id, IngredientUpdateDto ingredientUpdateDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<IngredientDto>> SearchAsync(string searchTerm);
    }
}
