using MyFood.Application;
using MyFood.Application.Entities;

namespace MyFood.Application.Services
{
    public interface IFoodService
    {
        IQueryable<FoodEntity> GetAll(QueryParameters queryParameters);
        FoodEntity? GetSingle(int id);
        IEnumerable<FoodEntity> SearchFoodsByName(string name);
        FoodEntity Add(FoodEntity item);
        FoodEntity Update(int id, FoodEntity item);
        void Delete(int id);
        ICollection<FoodEntity> GetRandomMeal();
        int Count();
    }
}