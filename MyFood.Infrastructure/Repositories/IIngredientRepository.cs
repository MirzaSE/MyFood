using MyFood.Application.Entities;
using MyFood.Application;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
       
            void Add(IngredientEntity item);
            void Delete(int id);
            IngredientEntity Update(int id, IngredientEntity item);
            IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters);
            int Count();
            bool Save();
        
    }
}