using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        IngredientEntity GetSingle(int id);
        void Add(IngredientEntity item);
        void Delete(int id);

        IngredientEntity Update(int id, IngredientEntity item);
        IEnumerable<IngredientEntity> GetByFoodId(int foodId);
        int Count();
        bool Save();
    }
}
