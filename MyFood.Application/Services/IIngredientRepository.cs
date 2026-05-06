using MyFood.Application;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IIngredientRepository
    {
        IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters);
        IngredientEntity? GetSingle(int id);
        IEnumerable<IngredientEntity> SearchByName(string name);
        bool ExistsByName(string name, int? excludingId = null);
        int Count(string? query = null);
        void Add(IngredientEntity entity);
        IngredientEntity Update(IngredientEntity entity);
        void Delete(IngredientEntity entity);
        bool Save();
    }
}
