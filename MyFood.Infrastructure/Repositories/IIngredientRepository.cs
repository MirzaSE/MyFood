using System.Collections.Generic;
using System.Threading.Tasks;
using MyFood.Api.Data.Entities;

namespace MyFood.Api.Data.Repositories
{
    public interface IIngredientRepository
    {
        Task<IngredientEntity> GetByIdAsync(int id);
        Task<IEnumerable<IngredientEntity>> GetAllAsync();
        Task AddAsync(IngredientEntity ingredient);
        Task UpdateAsync(IngredientEntity ingredient);
        Task DeleteAsync(int id);
    }
}