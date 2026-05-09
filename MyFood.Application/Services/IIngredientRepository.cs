using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IIngredientRepository
    {
        IngredientEntity? GetSingle(int id);
        IEnumerable<IngredientEntity> GetAll(QueryParameters queryParameters);
        IEnumerable<IngredientEntity> SearchByName(string name);
        void Add(IngredientEntity item);
        void Delete(int id);
        IngredientEntity Update(int id, IngredientEntity item);
        int Count();
        bool Save();
    }
}
