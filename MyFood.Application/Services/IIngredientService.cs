using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IIngredientService
    {
        Task<IEnumerable<IngredientEntity>> GetAllAsync(int? page = null, int? pageSize = null);
        Task<IngredientEntity?> GetByIdAsync(int id);
        Task<IngredientEntity> CreateAsync(IngredientEntity ingredient);
        Task<IngredientEntity?> UpdateAsync(int id, IngredientEntity ingredient);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<IngredientEntity>> SearchAsync(string query);
    }
}
