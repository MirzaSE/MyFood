using MyFood.Application.Entities;
using MyFood.Application;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
       
            void Add(IngredientEntity item);
            void Delete(int id);
            IngredientEntity Update(IngredientEntity item);
            IngredientEntity GetSingle(int id);
            IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters);

       
        bool Save();


    }
}