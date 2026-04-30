using MyFood.Domain.Entities;
using MyFood.Application;


namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
    IngredientEntity GetSingle(int id);
        void Add(IngredientEntity item);
        void Delete(int id);
        IngredientEntity Update(int id, IngredientEntity item);
        IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters);
        ICollection<IngredientEntity> GetRandomMeal();

        IEnumerable<IngredientEntity> SearchFoodsByName(string name);
        int Count();
        bool Save();
    }
}
