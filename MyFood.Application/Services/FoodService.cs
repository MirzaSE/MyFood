using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IIngredientRepository? _ingredientRepository;
        private readonly IMapper _mapper;

        public FoodService(IFoodRepository foodRepository, IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _foodRepository = foodRepository;
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        public FoodService(IFoodRepository foodRepository, IMapper mapper)
        {
            _foodRepository = foodRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FoodDto>> GetAllFoodsAsync(QueryParameters queryParameters)
        {
            var foodEntities = _foodRepository.GetAll(queryParameters);
            return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foodEntities));
        }

        public async Task<FoodDto?> GetFoodByIdAsync(int id)
        {
            var foodEntity = _foodRepository.GetSingle(id);
            return await Task.FromResult(foodEntity != null ? _mapper.Map<FoodDto>(foodEntity) : null);
        }

        public async Task<IEnumerable<FoodDto>> SearchFoodsByNameAsync(string name)
        {
            var foodEntities = _foodRepository.SearchFoodsByName(name);
            return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foodEntities));
        }

        public async Task<FoodDto> CreateFoodAsync(FoodCreateDto foodCreateDto)
        {
            var foodEntity = _mapper.Map<FoodEntity>(foodCreateDto);
            foodEntity.Created = DateTime.UtcNow;
            ApplyIngredientNutrition(foodCreateDto.Ingredients, foodEntity, shouldDeductStock: true);

            _foodRepository.Add(foodEntity);

            if (!_foodRepository.Save())
            {
                throw new Exception("Creating a food item failed on save.");
            }

            var newFoodEntity = _foodRepository.GetSingle(foodEntity.Id);
            return await Task.FromResult(_mapper.Map<FoodDto>(newFoodEntity));
        }

        public async Task<FoodDto?> UpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto)
        {
            var existingEntity = _foodRepository.GetSingle(id);
            if (existingEntity == null)
            {
                return null;
            }

            _mapper.Map(foodUpdateDto, existingEntity);
            ApplyIngredientNutrition(foodUpdateDto.Ingredients, existingEntity, shouldDeductStock: true);
            var updatedEntity = _foodRepository.Update(id, existingEntity);

            if (!_foodRepository.Save())
            {
                throw new Exception("Updating a food item failed on save.");
            }

            return await Task.FromResult(_mapper.Map<FoodDto>(updatedEntity));
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
            return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foodEntities));
        }

        public async Task<int> GetTotalFoodCountAsync()
        {
            return await Task.FromResult(_foodRepository.Count());
        }

        private void ApplyIngredientNutrition(
            IEnumerable<FoodIngredientSelectionDto>? selections,
            FoodEntity foodEntity,
            bool shouldDeductStock)
        {
            if (selections == null || !selections.Any())
            {
                return;
            }

            if (_ingredientRepository == null)
            {
                throw new InvalidOperationException("Ingredient repository is required to calculate food nutrition.");
            }

            double calories = foodEntity.Calories;
            double protein = foodEntity.Protein;
            double carbs = foodEntity.Carbs;
            double fat = foodEntity.Fat;

            foreach (var selection in selections)
            {
                if (selection.Quantity <= 0)
                {
                    throw new ArgumentException("Ingredient quantity must be greater than 0.", nameof(selections));
                }

                var ingredient = _ingredientRepository.GetSingle(selection.IngredientId);
                if (ingredient == null)
                {
                    throw new InvalidOperationException($"Ingredient {selection.IngredientId} was not found.");
                }

                if (selection.Quantity > ingredient.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Not enough {ingredient.Name}. Available: {ingredient.Quantity}, requested: {selection.Quantity}.");
                }

                calories += ingredient.CaloriesPerUnit * selection.Quantity;
                protein += ingredient.Protein * selection.Quantity;
                carbs += ingredient.Carbs * selection.Quantity;
                fat += ingredient.Fat * selection.Quantity;

                if (shouldDeductStock)
                {
                    ingredient.Quantity -= selection.Quantity;
                    _ingredientRepository.Update(ingredient.Id, ingredient);
                }
            }

            foodEntity.Calories = Math.Max(1, (int)Math.Round(calories));
            foodEntity.Protein = protein;
            foodEntity.Carbs = carbs;
            foodEntity.Fat = fat;
        }
    }
}
