using MyFood.Application.Dtos;

namespace MyFood.Application.Services
{
    public interface IIngredientService
    {
        Task<IEnumerable<IngredientDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<IngredientDto?> GetByIdAsync(int id);
        Task<IngredientDto> CreateAsync(CreateIngredientDto dto);
        Task<IngredientDto?> UpdateAsync(int id, UpdateIngredientDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<IngredientDto>> SearchAsync(string searchTerm);
    }
}