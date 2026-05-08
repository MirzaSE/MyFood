using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IIngredientRepository
    {
        IngredientEntity? GetSingle(int id);
        void Add(IngredientEntity item);
        void Delete(int id);
        IngredientEntity Update(int id, IngredientEntity item);
        IQueryable<IngredientEntity> GetAll();
        IEnumerable<IngredientEntity> GetByFoodId(int foodEntityId);
        IEnumerable<IngredientEntity> SearchIngredientsByName(string name);
        int Count();
        bool Save();
    }
}