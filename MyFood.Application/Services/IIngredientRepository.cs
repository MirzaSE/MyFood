using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IIngredientRepository
    {
        IngredientEntity GetSingle(int id);
        void Add(IngredientEntity item);
        void Delete(int id);
        IngredientEntity Update(int id, IngredientEntity item);
        IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters);
        int Count();
        bool Save();
        Task<IEnumerable<IngredientEntity>> GetAllAsync();
        Task AddAsync(IngredientEntity ingredient);
        Task DeleteAsync(int id);
    }
}
