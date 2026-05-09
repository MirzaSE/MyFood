using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IIngredientRepository
    {
        IQueryable<IngredientEntity> GetAll();
        IQueryable<IngredientEntity> SearchByName(string name);
        IngredientEntity? GetSingle(int id);
        IngredientEntity? GetByExactName(string name);
        void Add(IngredientEntity item);
        IngredientEntity Update(int id, IngredientEntity item);
        void Delete(int id);
        int Count();
        bool Save();
    }
}
