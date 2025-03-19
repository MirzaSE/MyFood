using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Helpers;

namespace MyFood.Infrastructure.Repositories
{
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

        Task<ServiceResponse<FoodEntity>> AddIngredientsToFood(int foodId, List<IngredientLinkToFoodDto> ingredientDtos);
    }
}
