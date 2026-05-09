using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IIngredientRepository
    {
        IQueryable<IngredientEntity> GetAll(QueryParameters parameters);
        IngredientEntity? GetSingle(int id);
        Task<IngredientEntity?> GetByNameAsync(string name);
        Task<IngredientEntity> CreateAsync(IngredientEntity ingredient);
        Task<IngredientEntity> UpdateAsync(IngredientEntity ingredient);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<IngredientEntity>> SearchAsync(string searchTerm);
    }
}
