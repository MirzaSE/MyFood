using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        IngredientEntity? GetSingle(int id);
        IEnumerable<IngredientEntity> GetAll();
        IEnumerable<IngredientEntity> GetByFoodId(int foodId);
        void Add(IngredientEntity ingredient);  
        IngredientEntity Update(int id, IngredientEntity ingredient);
        void Delete(int id);
        bool Save();
    }
}