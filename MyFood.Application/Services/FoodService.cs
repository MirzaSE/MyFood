using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IMapper _mapper;

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

            // Handle ingredients if provided
            if (foodCreateDto.Ingredients != null && foodCreateDto.Ingredients.Any())
            {
                foodEntity.FoodIngredients = foodCreateDto.Ingredients
                    .Select(i => new FoodIngredient
                    {
                        IngredientId = i.IngredientId,
                        Quantity = i.Quantity
                    })
                    .ToList();
            }

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
    }
}