using MyFood.Application.Dtos;

namespace MyFood.Application.Services
{
    public interface IIngredientService
    {
        Task<IEnumerable<IngredientDto>> GetAllAsync(QueryParameters queryParameters);
        Task<IngredientDto?> GetByIdAsync(int id);
        Task<IEnumerable<IngredientDto>> SearchAsync(string name);
        Task<IngredientDto> CreateAsync(IngredientCreateDto createDto);
        Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<int> GetTotalCountAsync();
    }
}
