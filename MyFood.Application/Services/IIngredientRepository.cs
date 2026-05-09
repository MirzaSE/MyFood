using MyFood.Domain.Entities;

namespace MyFood.Application.Services;

public interface IIngredientRepository
{
    IEnumerable<IngredientEntity> GetAll();
    IngredientEntity? GetById(int id);
    IngredientEntity Add(IngredientEntity ingredient);
    IngredientEntity Update(int id, IngredientEntity ingredient);
    void Delete(int id);
    bool Save();
}
