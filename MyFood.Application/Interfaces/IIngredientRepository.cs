using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFood.Domain.Entities;


namespace MyFood.Application.Interfaces
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<IngredientEntity>> GetAllAsync();
        Task<IngredientEntity?> GetByIdAsync(int id);
        Task AddAsync(IngredientEntity ingredient);
        Task UpdateAsync(IngredientEntity ingredient);
        Task DeleteAsync(int id);
    }
}
