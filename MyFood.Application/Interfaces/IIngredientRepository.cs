using MyFood.Domain.Entities;

namespace MyFood.Application.Interfaces
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<IngredientEntity>> GetAllAsync();
        Task<IngredientEntity?> GetByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByNameExceptIdAsync(string name, int exceptId);
        Task<IngredientEntity> CreateAsync(IngredientEntity ingredient);
        Task UpdateAsync(IngredientEntity ingredient);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<IngredientEntity>> SearchAsync(string normalizedQueryLower);

        void AddIngredient(IngredientEntity ingredient);
        IEnumerable<IngredientEntity> GetAllIngredients();
        IngredientEntity? GetIngredientById(int id);
        void UpdateIngredient(IngredientEntity ingredient);
        void DeleteIngredient(int id);
    }
}
