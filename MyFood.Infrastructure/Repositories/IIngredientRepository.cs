using MyFood.Application.Entities;
using System.Collections.Generic;

namespace MyFood.Application.Interfaces
{
    public interface IIngredientRepository
    {
        void Add(IngredientEntity ingredient);
        void Update(IngredientEntity ingredient);
        void Delete(int id);
        IngredientEntity? GetById(int id);
        List<IngredientEntity> GetAll();
    }
}