using MyFood.Application;
using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        Task<IngredientEntity?> GetSingle(int id);
        Task<IngredientEntity> Add(IngredientEntity item);
        Task<IngredientEntity?> Delete(int id);
        Task<IngredientEntity> Update(int id, IngredientEntity item);
        Task<IEnumerable<IngredientEntity>> GetAll(QueryParameters queryParameters);
        Task<int> Count();
    }
}
