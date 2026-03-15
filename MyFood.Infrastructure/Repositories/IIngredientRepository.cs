using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        IEnumerable<IngredientEntity> GetAllForFood(int foodId);
        IngredientEntity? GetSingle(int foodId, int ingredientId);
        void Add(IngredientEntity item);
        void Update(IngredientEntity item);
        void Delete(IngredientEntity item);
        bool Save();
    }
}
