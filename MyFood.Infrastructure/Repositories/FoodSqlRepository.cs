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
            _foodDbContext = foodDbContext ?? throw new ArgumentNullException(nameof(foodDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public FoodEntity GetSingle(int id)
        {
            return _foodDbContext.FoodItems
                .Include(f => f.Ingredients)
                .FirstOrDefault(x => x.Id == id);
        }

        public void Add(FoodEntity item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _foodDbContext.FoodItems.Add(item);
        }

        public void Delete(int id)
        {
            var foodItem = GetSingle(id);
            if (foodItem != null)
            {
                _foodDbContext.FoodItems.Remove(foodItem);
            }
        }

        public FoodEntity Update(int id, FoodEntity item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _foodDbContext.FoodItems.Update(item);
            return item;
        }

        public IQueryable<FoodEntity> GetAll(QueryParameters queryParameters)
        {
            var query = _foodDbContext.FoodItems
                .OrderBy(x => x.Name)
                .AsQueryable();

            if (queryParameters.HasQuery())
            {
                query = query.Where(x => x.Calories.ToString().Contains(queryParameters.Query.ToLowerInvariant())
                              || x.Name.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant()));
            }

            return query
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public int Count()
        {
            return _foodDbContext.FoodItems.Count();
        }

        public bool Save()
        {
            return _foodDbContext.SaveChanges() >= 0;
        }

        public async Task<ServiceResponse<FoodEntity>> AddIngredientsToFood(int foodId, List<IngredientLinkToFoodDto> ingredientDtos)
        {
            if (ingredientDtos == null)
            {
                throw new ArgumentNullException(nameof(ingredientDtos));
            }

            var serviceResponse = new ServiceResponse<FoodEntity>();
            var foodEntity = await _foodDbContext.FoodItems
                .Include(f => f.Ingredients)
                .FirstOrDefaultAsync(x => x.Id == foodId);

            if (foodEntity == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "No food found.";
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
            return new List<FoodEntity>
            {
                GetRandomItem("Starter"),
                GetRandomItem("Main"),
                GetRandomItem("Dessert")
            };
        }

        public IEnumerable<FoodEntity> SearchFoodsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            return _foodDbContext.FoodItems
                .Where(f => EF.Functions.Like(f.Name, $"%{name}%"))
                .ToList();
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