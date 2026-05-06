using MyFood.Application.Interfaces;
using MyFood.Application.Entities;

namespace MyFood.Application.Interfaces
{
    public interface IIngredientRepository
    {
        void AddIngredient(IngredientEntity ingredient);
        IEnumerable<IngredientEntity> GetAllIngredients();
        IngredientEntity? GetIngredientById(int id);
        void UpdateIngredient(IngredientEntity ingredient);
        void DeleteIngredient(int id);
    }
}