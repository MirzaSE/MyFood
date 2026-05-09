using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public FoodService(IFoodRepository foodRepository, IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _foodRepository = foodRepository;
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FoodDto>> GetAllFoodsAsync(QueryParameters queryParameters)
        {
            var foodEntities = _foodRepository.GetAll(queryParameters).ToList();
            return await Task.FromResult(foodEntities.Select(MapFoodDto));
        }

        public async Task<FoodDto?> GetFoodByIdAsync(int id)
        {
            var foodEntity = _foodRepository.GetSingle(id);
            return await Task.FromResult(foodEntity != null ? MapFoodDto(foodEntity) : null);
        }

        public async Task<IEnumerable<FoodDto>> SearchFoodsByNameAsync(string name)
        {
            var foodEntities = _foodRepository.SearchFoodsByName(name);
            return await Task.FromResult(foodEntities.Select(MapFoodDto));
        }

        public async Task<FoodDto> CreateFoodAsync(FoodCreateDto foodCreateDto)
        {
            var foodEntity = _mapper.Map<FoodEntity>(foodCreateDto);
            foodEntity.Created = foodCreateDto.Created == default ? DateTime.UtcNow : foodCreateDto.Created;
            foodEntity.FoodIngredients = BuildFoodIngredients(foodCreateDto.Ingredients);
            foodEntity.Calories = CalculateCalories(foodCreateDto.Calories, foodEntity.FoodIngredients);
            _foodRepository.Add(foodEntity);

            if (!_foodRepository.Save())
            {
                throw new Exception("Creating a food item failed on save.");
            }

            var newFoodEntity = _foodRepository.GetSingle(foodEntity.Id);
            return await Task.FromResult(MapFoodDto(newFoodEntity!));
        }

        public async Task<FoodDto?> UpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto)
        {
            var existingEntity = _foodRepository.GetSingle(id);
            if (existingEntity == null)
            {
                return null;
            }

            _mapper.Map(foodUpdateDto, existingEntity);
            existingEntity.FoodIngredients.Clear();

            foreach (var item in BuildFoodIngredients(foodUpdateDto.Ingredients))
            {
                existingEntity.FoodIngredients.Add(item);
            }

            existingEntity.Calories = CalculateCalories(foodUpdateDto.Calories, existingEntity.FoodIngredients);
            var updatedEntity = _foodRepository.Update(id, existingEntity);

            if (!_foodRepository.Save())
            {
                throw new Exception("Updating a food item failed on save.");
            }

            return await Task.FromResult(MapFoodDto(updatedEntity));
        }

        public async Task<bool> DeleteFoodAsync(int id)
        {
            var foodEntity = _foodRepository.GetSingle(id);
            if (foodEntity == null)
            {
                return false;
            }

            _foodRepository.Delete(id);

            if (!_foodRepository.Save())
            {
                throw new Exception("Deleting a food item failed on save.");
            }

            return true;
        }

        public async Task<IEnumerable<FoodDto>> GetRandomMealAsync()
        {
            var foodEntities = _foodRepository.GetRandomMeal();
            return await Task.FromResult(foodEntities.Select(MapFoodDto));
        }

        public async Task<int> GetTotalFoodCountAsync()
        {
            return await Task.FromResult(_foodRepository.Count());
        }

        private List<FoodIngredientEntity> BuildFoodIngredients(IEnumerable<FoodIngredientInputDto>? ingredients)
        {
            var result = new List<FoodIngredientEntity>();
            if (ingredients == null)
            {
                return result;
            }

            foreach (var item in ingredients.Where(x => x.Quantity > 0))
            {
                var ingredient = _ingredientRepository.GetSingle(item.IngredientId);
                if (ingredient == null)
                {
                    throw new ArgumentException($"Ingredient with id {item.IngredientId} was not found.");
                }

                result.Add(new FoodIngredientEntity
                {
                    IngredientEntityId = ingredient.Id,
                    Ingredient = ingredient,
                    Quantity = item.Quantity
                });
            }

            return result;
        }

        private static int CalculateCalories(int fallbackCalories, IEnumerable<FoodIngredientEntity> ingredients)
        {
            var calculated = ingredients.Sum(x => x.Quantity * x.Ingredient.CaloriesPerUnit);
            return calculated > 0 ? (int)Math.Round(calculated, MidpointRounding.AwayFromZero) : fallbackCalories;
        }

        private FoodDto MapFoodDto(FoodEntity entity)
        {
            var dto = _mapper.Map<FoodDto>(entity);
            dto.Ingredients = entity.FoodIngredients
                .Where(x => x.Ingredient != null)
                .Select(x => _mapper.Map<FoodIngredientDto>(x))
                .ToList();

            dto.NutritionTotals = new NutritionTotalsDto
            {
                Calories = dto.Ingredients.Sum(x => x.Calories),
                Protein = dto.Ingredients.Sum(x => x.Protein),
                Carbs = dto.Ingredients.Sum(x => x.Carbs),
                Fat = dto.Ingredients.Sum(x => x.Fat)
            };

            if (dto.NutritionTotals.Calories == 0)
            {
                dto.NutritionTotals.Calories = entity.Calories;
            }

            return dto;
        }
    }
}
