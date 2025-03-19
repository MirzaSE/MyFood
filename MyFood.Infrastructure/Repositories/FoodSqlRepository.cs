using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Helpers;

namespace MyFood.Infrastructure.Repositories
{
    public class FoodSqlRepository : IFoodRepository
    {
        private readonly FoodDbContext _foodDbContext;
        private readonly IMapper _mapper;

        public FoodSqlRepository(FoodDbContext foodDbContext, IMapper mapper)
        {
            _foodDbContext = foodDbContext;
            _mapper = mapper;
        }

        public FoodEntity GetSingle(int id)
        {
            return _foodDbContext.FoodItems.FirstOrDefault(x => x.Id == id);
        }

        public void Add(FoodEntity item)
        {
            _foodDbContext.FoodItems.Add(item);
        }

        public void Delete(int id)
        {
            FoodEntity foodItem = GetSingle(id);
            _foodDbContext.FoodItems.Remove(foodItem);
        }

        public FoodEntity Update(int id, FoodEntity item)
        {
            _foodDbContext.FoodItems.Update(item);
            return item;
        }

        public IQueryable<FoodEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<FoodEntity> _allItems = _foodDbContext.FoodItems.OrderBy(x => x.Name);

            if (queryParameters.HasQuery())
            {
                _allItems = _allItems
                    .Where(x => x.Calories.ToString().Contains(queryParameters.Query.ToLowerInvariant())
                                || x.Name.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant()));
            }

            return _allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public int Count()
        {
            return _foodDbContext.FoodItems.Count();
        }

        public bool Save()
        {
            return (_foodDbContext.SaveChanges() >= 0);
        }

        public async Task<ServiceResponse<FoodEntity>> AddIngredientsToFood(int foodId,
            List<IngredientLinkToFoodDto> ingredientDtos)
        {
            var serviceResponse = new ServiceResponse<FoodEntity>();
            var foodEntity = await _foodDbContext.FoodItems
                .Include(f => f.Ingredients)
                .FirstOrDefaultAsync(x => x.Id == foodId);
            if (foodEntity == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "No food found";
                return serviceResponse;
            }

            var ingredients = _mapper.Map<List<IngredientEntity>>(ingredientDtos);
            foreach (var ingredient in ingredients)
            {
                ingredient.FoodEntityId = foodId;
            }

            foodEntity.Ingredients = ingredients;
            await _foodDbContext.SaveChangesAsync();

            serviceResponse.Data = foodEntity;
            serviceResponse.Success = true;
            return serviceResponse;
        }

        public ICollection<FoodEntity> GetRandomMeal()
        {
            List<FoodEntity> toReturn = new List<FoodEntity>();

            toReturn.Add(GetRandomItem("Starter"));
            toReturn.Add(GetRandomItem("Main"));
            toReturn.Add(GetRandomItem("Dessert"));

            return toReturn;
        }


        public IEnumerable<FoodEntity> SearchFoodsByName(string name)
        {
            return _foodDbContext.FoodItems
                .Where(f => EF.Functions.Like(f.Name, $"%{name}%"))
                .ToList();

            // SELECT * FROM FoodItems WHERE Name LIKE '%name%'
        }

        private FoodEntity GetRandomItem(string type)
        {
            return _foodDbContext.FoodItems
                .Where(x => x.Type == type)
                .OrderBy(o => Guid.NewGuid())
                .FirstOrDefault();
        }
    }
}