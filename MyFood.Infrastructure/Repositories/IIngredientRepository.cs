using MyFood.Domain.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        IngredientEntity GetSingle(int id);
        void Add(IngredientEntity item);
        void Delete(int id);
        IngredientEntity Update(int id, IngredientEntity item);
        IQueryable<IngredientEntity> GetAll();
        IEnumerable<IngredientEntity> GetByFoodId(int foodId);
        int Count();
        bool Save();
    }
}
