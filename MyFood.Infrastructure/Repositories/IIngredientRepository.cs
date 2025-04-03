using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        IngredientEntity GetSingle(int id);
        void Add(IngredientEntity ingredient);
        void Delete(int id);
        IngredientEntity Update(int id, IngredientEntity ingredient);
        IQueryable<IngredientEntity> GetAll();
        int Count();
        bool Save();
    }
}
