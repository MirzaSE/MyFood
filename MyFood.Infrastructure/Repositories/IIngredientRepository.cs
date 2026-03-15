using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        IngredientEntity GetSingle(int id);
        IEnumerable<IngredientEntity> GetAllForFood(int foodId);
        void Add(IngredientEntity item);
        IngredientEntity Update(int id, IngredientEntity item);
        void Delete(int id);
        bool Save();
    }
}
