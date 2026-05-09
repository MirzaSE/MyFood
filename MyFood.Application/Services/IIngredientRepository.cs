using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IIngredientRepository
    {
        IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters);
        Task<IngredientEntity?> GetByIdAsync(int id);
        Task<IngredientEntity> AddAsync(IngredientEntity ingredient);
        Task<IngredientEntity?> UpdateAsync(int id, IngredientEntity ingredient);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<IngredientEntity>> SearchAsync(string name);
        Task<int> CountAsync();
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}
