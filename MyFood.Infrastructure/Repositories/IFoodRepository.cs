
using MyFood.Application.Entities;
using MyFood.Application;

namespace MyFood.Infrastructure.Repositories
{
    public interface IFoodRepository
    {
        FoodEntity GetSingle(int id);
        void Add(FoodEntity item);
        void Delete(int id);
        FoodEntity Update(int id, FoodEntity item);
        IQueryable<FoodEntity> GetAll(QueryParameters queryParameters);
        IQueryable<FoodEntity> SearchFoodsByName(string name);
        ICollection<FoodEntity> GetRandomMeal();
        int Count();
        bool Save();
    }
}