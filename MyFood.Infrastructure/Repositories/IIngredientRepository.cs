using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFood.Application.Entities;


namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
    Task<List<IngredientEntity>> GetAllAsync();

    Task<IngredientEntity?> GetByIdAsync(int id);

    Task<IngredientEntity> AddAsync(IngredientEntity ingredient);

    Task<IngredientEntity> UpdateAsync(IngredientEntity ingredient);

    Task DeleteAsync(int id);
    }
}