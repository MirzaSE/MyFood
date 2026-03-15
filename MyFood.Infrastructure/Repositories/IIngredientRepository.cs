using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        ICollection<IngredientEntity> GetAll();
        IngredientEntity GetSingle(int id);
        ICollection<IngredientEntity> Search(string name);
        void Add(IngredientEntity item);
        void Delete(int id);
        IngredientEntity Update(IngredientEntity item);
        bool Save();
    }
}