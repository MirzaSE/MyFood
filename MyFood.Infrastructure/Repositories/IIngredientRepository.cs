using MyFood.Application.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFood.Application.Repositories
{
    public interface IIngredientRepository
    {
        Task AddAsync(IngredientEntity ingredient);
        Task UpdateAsync(IngredientEntity ingredient);
        Task DeleteAsync(int ingredientId);
        Task<IngredientEntity> GetByIdAsync(int ingredientId);
        Task<IEnumerable<IngredientEntity>> GetAllAsync();
        Task<bool> SaveAsync(); // for saving changes asynchronously
    }
}
