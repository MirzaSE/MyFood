using MyFood.Domain.Entities;

namespace MyFood.Application.Repositories
{
    public interface IIngredientRepository
    {
        IQueryable<IngredientEntity> GetAll();
        IngredientEntity? GetSingle(int id);
        IQueryable<IngredientEntity> GetByFoodId(int foodId);
        void Add(IngredientEntity item);
        IngredientEntity Update(int id, IngredientEntity item);
        void Delete(int id);
        bool Save();
    }
}