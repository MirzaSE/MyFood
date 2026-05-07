using MyFood.Application;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IFoodRepository
    {
        FoodEntity? GetSingle(int id);
        void Add(FoodEntity item);
        void Delete(int id);
        FoodEntity Update(int id, FoodEntity item);
        IQueryable<FoodEntity> GetAll(QueryParameters queryParameters);
        ICollection<FoodEntity> GetRandomMeal();

        IEnumerable<FoodEntity> SearchFoodsByName(string name);
        int Count();
        bool Save();
    }
}