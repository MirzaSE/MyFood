using MyFood.Domain.Entities;

namespace MyFood.Application.Services
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