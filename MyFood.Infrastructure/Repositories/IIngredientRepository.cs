using MyFood.Application;
using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        Task<List<IngredientEntity>> GetAllAsync();

        Task<IngredientEntity?> GetByIdAsync(int id);

        Task<IngredientEntity> AddAsync(IngredientEntity ingredient);

        Task<IngredientEntity?> UpdateAsync(int id, IngredientEntity ingredient);

        Task<bool> DeleteAsync(int id);
    }
}