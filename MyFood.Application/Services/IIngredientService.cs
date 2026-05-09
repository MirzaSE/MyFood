using MyFood.Application.Dtos;

namespace MyFood.Application.Services;

public interface IIngredientService
{
    Task<IEnumerable<IngredientDto>> GetAllAsync(QueryParameters queryParameters);
    Task<IngredientDto?> GetByIdAsync(int id);
    Task<IngredientDto> CreateAsync(IngredientCreateDto dto);
    Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<IngredientDto>> SearchAsync(string name);
    Task<int> GetTotalCountAsync();
}
