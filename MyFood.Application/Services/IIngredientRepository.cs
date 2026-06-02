using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IIngredientRepository
    {
        IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters);
        IngredientEntity? GetSingle(int id);
        void Add(IngredientEntity item);
        IngredientEntity Update(IngredientEntity item);
        void Delete(IngredientEntity item);
        bool ExistsByName(string name);
        IEnumerable<IngredientEntity> SearchByName(string name);
        int Count();
        bool Save();
    }
}
