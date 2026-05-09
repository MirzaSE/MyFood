using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IIngredientRepository
    {
        IQueryable<IngredientEntity> GetAll();
        IngredientEntity? GetSingle(int id);
        void Add(IngredientEntity ingredient);
        IngredientEntity Update(int id, IngredientEntity ingredient);
        void Delete(int id);
        bool Save();
    }
}