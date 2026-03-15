using MyFood.Application.Entities;

namespace MyFood.Application.Interfaces
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<IngredientEntity>> GetAllAsync();
        Task<IngredientEntity?> GetByIdAsync(int id);
        Task<IngredientEntity> AddAsync(IngredientEntity ingredient);
        Task<IngredientEntity?> UpdateAsync(int id, IngredientEntity ingredient);
        Task<bool> DeleteAsync(int id);
    }
}