using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        IQueryable<IngredientEntity> GetAll();
        IngredientEntity GetSingle(int id);
        void Add(IngredientEntity item);
        IngredientEntity Update(int id, IngredientEntity item);
        void Delete(int id);
        bool Save();
    }
}
