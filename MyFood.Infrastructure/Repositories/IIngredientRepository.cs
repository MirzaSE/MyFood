using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        IQueryable<IngredientEntity> GetAll();
        IngredientEntity GetSingle(int id);
        void Add(IngredientEntity item);
        void Delete(IngredientEntity item);
        IngredientEntity Update(IngredientEntity item);
        bool Save();
    }
}