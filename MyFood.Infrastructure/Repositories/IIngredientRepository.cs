using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        IngredientEntity GetSingle(int id);
        IEnumerable<IngredientEntity> GetAll();
        IEnumerable<IngredientEntity> GetByFoodId(int foodId);
        void Add(IngredientEntity item);
        void Delete(int id);
        IngredientEntity Update(int id, IngredientEntity item);
        bool Save();
    }
}