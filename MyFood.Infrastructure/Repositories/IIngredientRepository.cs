using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        Task<IngredientEntity?> GetIngredientByIdAsync(int id);
        Task<IEnumerable<IngredientEntity>> ListIngredientsAsync();
        Task AddIngredientAsync(IngredientEntity ingredient);
        Task UpdateIngredientAsync(IngredientEntity ingredient);
        Task DeleteIngredientAsync(IngredientEntity ingredient);
    }
}