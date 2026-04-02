using MyFood.Application;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services;

public interface IFoodRepository
{
    FoodEntity GetSingle(int id);
    void Add(FoodEntity item);
    void Delete(int id);
    FoodEntity Update(int id, FoodEntity item);
    IQueryable<FoodEntity> GetAll(QueryParameters queryParameters);
    ICollection<FoodEntity> GetRandomMeal();

    IEnumerable<FoodEntity> SearchFoodsByName(string name);
    int Count();
    bool Save();


    Task<FoodEntity?> GetSingleAsync(int id);
    Task AddAsync(FoodEntity item);
    Task DeleteAsync(int id);
    Task<FoodEntity?> UpdateAsync(int id, FoodEntity item);
    Task<IEnumerable<FoodEntity>> GetAllAsync(QueryParameters queryParameters);
    Task<ICollection<FoodEntity>> GetRandomMealAsync();

    Task<IEnumerable<FoodEntity>> SearchFoodsByNameAsync(string name);
    Task<bool> SaveAsync();
    Task<int> CountAsync();
}
