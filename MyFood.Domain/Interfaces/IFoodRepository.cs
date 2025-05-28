using MyFood.Domain.Entities;

namespace MyFood.Domain.Interfaces
{
    public interface IFoodRepository
    {
        FoodEntity GetSingle(int id);
        void Add(FoodEntity item);
        void Delete(int id);
        FoodEntity Update(int id, FoodEntity item);
        IQueryable<FoodEntity> GetAll(string query, int pageCount, int page);
        ICollection<FoodEntity> GetRandomMeal();

        IEnumerable<FoodEntity> SearchFoodsByName(string name);
        int Count();
        bool Save();
    }
}
