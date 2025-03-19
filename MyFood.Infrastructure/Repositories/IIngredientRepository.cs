using MyFood.Application.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFood.Application.Interfaces
{
    public interface IIngredientRepository
    {
        void AddIngredient(IngredientEntity ingredient);
        void UpdateIngredient(IngredientEntity ingredient);
        void DeleteIngredient(int ingredientId);
        List<IngredientEntity> GetAllIngredients();
        List<IngredientEntity> GetIngredientsByFoodId(int foodId);
    }
}
