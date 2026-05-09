using MyFood.Domain.Entities;

namespace MyFood.Application
{
    public interface IIngredientRepository
    {
        Task<List<IngredientEntity>> GetAllAsync(QueryParameters queryParameters);
        Task<int> CountAsync();
        Task<IngredientEntity?> GetByIdAsync(int id);
        Task<IngredientEntity> AddAsync(IngredientEntity ingredient);
        Task<IngredientEntity?> UpdateAsync(int id, IngredientEntity ingredient);
        Task<bool> DeleteAsync(int id);
        Task<List<IngredientEntity>> SearchAsync(string name);
        Task<bool> ExistsAsync(string name);
    }
}
