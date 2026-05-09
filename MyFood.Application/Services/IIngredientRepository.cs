using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IIngredientRepository
    {
        IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters);
        IngredientEntity? GetSingle(int id);
        IEnumerable<IngredientEntity> SearchByName(string name);
        bool ExistsByName(string name, int? excludingId = null);
        void Add(IngredientEntity item);
        IngredientEntity Update(int id, IngredientEntity item);
        void Delete(int id);
        int Count();
        bool Save();
    }
}
