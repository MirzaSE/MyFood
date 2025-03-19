
using MyFood.Application.Entities;  // Ensure you have this namespace for IngredientEntity
using System.Collections.Generic;
using System.Linq;
public interface IIngredientRepository
{
    void AddIngredient(IngredientEntity ingredient);
    IEnumerable<IngredientEntity> GetAllIngredients();
    IngredientEntity GetIngredientById(int id);
    void UpdateIngredient(IngredientEntity ingredient);
    void DeleteIngredient(int id);
}
