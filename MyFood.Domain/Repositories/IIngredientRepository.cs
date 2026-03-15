using System.Collections.Generic;
using System.Threading.Tasks;
using MyFood.Domain.Entities;

namespace MyFood.Domain.Repositories
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<IngredientEntity>> GetAllAsync();
        Task<IngredientEntity?> GetByIdAsync(int id);
        Task<IngredientEntity> AddAsync(IngredientEntity ingredient);
        Task<IngredientEntity> UpdateAsync(IngredientEntity ingredient);
        Task DeleteAsync(int id);
    }
}