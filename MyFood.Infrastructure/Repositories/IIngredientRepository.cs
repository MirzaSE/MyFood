using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        IngredientEntity GetSingle(int id);
        IEnumerable<IngredientEntity> GetAllByFoodId(int foodId);
        void Add(int foodId, IngredientEntity ingredient);
        IngredientEntity Update(int id, IngredientEntity ingredient);
        void Delete(int id);
        bool FoodExists(int foodId);
        int Count(int foodId);
        bool Save();
    }
}