using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::MyFood.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Repositories;


namespace MyFood.Application.Repositories
{
    public interface IIngredientRepository
    {
        // Method for adding a new ingredient
        Task<IngredientEntity> AddAsync(IngredientEntity ingredient);

        // Method for updating an existing ingredient
        Task<IngredientEntity> UpdateAsync(IngredientEntity ingredient);

        // Method for deleting an ingredient by its ID
        Task<bool> DeleteAsync(int ingredientId);

        // Method for retrieving an ingredient by its ID
        Task<IngredientEntity> GetByIdAsync(int ingredientId);

        // Method for retrieving all ingredients
        Task<IEnumerable<IngredientEntity>> GetAllAsync();

        // Method for finding ingredients by name (optional)
        Task<IEnumerable<IngredientEntity>> GetByNameAsync(string name);
    }
}

