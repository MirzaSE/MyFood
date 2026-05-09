using MyFood.Application.Dtos;

namespace MyFood.Application.Services
{
    public interface IIngredientService
    {
        Task<IEnumerable<IngridientDto>> GetAllAsync(QueryParameters queryParameters);
        Task<IngridientDto?> GetByIdAsync(int id);
        Task<IngridientDto> CreateAsync(IngridientCreateDto createDto);
        Task<IngridientDto?> UpdateAsync(int id, IngridientUpdate updateDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<IngridientDto>> SearchAsync(string name);
    }
}
