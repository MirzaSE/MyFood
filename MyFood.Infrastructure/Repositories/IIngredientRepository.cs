using MyFood.Application.Entities;

namespace MyFood.Application.Repositories
{
    public interface IIngredientRepository
    {
        IngredientEntity GetSingle(int id);
        void Add(IngredientEntity ingredient);
        void Delete(int id);
        void Update(IngredientEntity ingredient);
        IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters);
        bool Save();
        int Count();
    }
}