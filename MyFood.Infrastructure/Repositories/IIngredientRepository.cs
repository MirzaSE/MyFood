using MyFood.Application;
using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
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
        IEnumerable<IngredientEntity> SearchIngredientsByName(string name);
    }
}