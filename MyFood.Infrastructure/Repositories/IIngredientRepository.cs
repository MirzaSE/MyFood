using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
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
