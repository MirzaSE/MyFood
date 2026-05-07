using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IIngredientRepository
    {
        IQueryable<IngredientEntity> GetAll();
        IngredientEntity? GetById(int id);
        IngredientEntity? GetByName(string name);
        void Add(IngredientEntity ingredient);
        void Update(IngredientEntity ingredient);
        void Delete(IngredientEntity ingredient);
        bool Save();
    }
}