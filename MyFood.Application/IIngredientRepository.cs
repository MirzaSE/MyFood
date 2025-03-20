using System.Collections.Generic;
using System.Threading.Tasks;
using MyFood.Domain.Entities;

namespace MyFood.Application
{
    public interface IIngredientRepository
    {
        Task<IngredientEntity> AddAsync(IngredientEntity ingredient);
        Task<IngredientEntity?> GetByIdAsync(int id);
        Task<IEnumerable<IngredientEntity>> GetAllByFoodIdAsync(int foodId);
        Task UpdateAsync(IngredientEntity ingredient);
        Task DeleteAsync(int id);
    }
}