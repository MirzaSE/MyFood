using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        void AddIngredient(IngredientEntity ingredient);
        IEnumerable<IngredientEntity> GetAllIngredients();
        IngredientEntity? GetIngredientById(int id);
        void UpdateIngredient(IngredientEntity ingredient);
        void DeleteIngredient(int id);
        bool Save();
    }
}