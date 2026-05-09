using MyFood.Application;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IIngredientRepository
    {
        IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters);
        IEnumerable<IngredientEntity> Search(string query);
        IngredientEntity? GetById(int id);
        IEnumerable<IngredientEntity> GetAllForFood(int foodId);
        IngredientEntity? GetSingle(int foodId, int ingredientId);
        void Add(IngredientEntity item);
        void Update(IngredientEntity item);
        void Delete(IngredientEntity item);
        bool Save();
    }
}
