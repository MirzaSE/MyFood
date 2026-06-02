using System.Collections.Generic;
using MyFood.Application.Entities;

namespace MyFood.Infrastructure.Repositories
{
    public interface IIngredientRepository
    {
        IEnumerable<IngredientEntity> GetAll();
        IngredientEntity GetSingle(int id);
        IEnumerable<IngredientEntity> GetByFoodId(int foodId);
        void Add(IngredientEntity ingredient);
        IngredientEntity Update(int id, IngredientEntity ingredient);
        void Delete(int id);
        bool Save();
    }
}
