using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        Task AddIngredientAsync(IngredientEntity ingredient);
        Task UpdateIngredientAsync(IngredientEntity ingredient);
        Task DeleteIngredientAsync(int id);
        Task<IngredientEntity> GetIngredientAsync(int id);
        Task<List<IngredientEntity>> GetAllIngredientsAsync();
    }
}