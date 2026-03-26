using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        IngredientEntity GetSingle(int id);
        IQueryable<IngredientEntity> GetAll();
        IQueryable<IngredientEntity> GetByFoodId(int foodId);
        void Add(IngredientEntity item);
        void Delete(int id);
        IngredientEntity Update(int id, IngredientEntity item);
        bool Save();
    }
}
