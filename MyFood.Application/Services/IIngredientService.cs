using MyFood.Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFood.Application.Services
{
    public interface IIngredientService
    {
        Task<(IEnumerable<IngredientDto> Data, int TotalCount)> GetAllAsync(QueryParameters queryParameters);
        Task<IngredientDto?> GetByIdAsync(int id);
        Task<IngredientDto> CreateAsync(IngredientCreateDto createDto);
        Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<IngredientDto>> SearchAsync(string searchTerm);
    }
}