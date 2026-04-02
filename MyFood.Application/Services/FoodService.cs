using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using MyFood.Application;
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
            var foodEntities = await _foodRepository.GetAllAsync(queryParameters);
            return _mapper.Map<IEnumerable<FoodDto>>(foodEntities);
        }

        public async Task<FoodDto?> GetFoodByIdAsync(int id)
        {
            if (id < 0)
            {
                return null;
            }

            var foodItem = await _foodRepository.GetSingleAsync(id);
            if (foodItem == null)
            {
                return null;
            }

            return _mapper.Map<FoodDto>(foodItem);
        }

        public async Task<IEnumerable<FoodDto>> SearchFoodsByNameAsync(string name)
        {
            var foodEntities = await _foodRepository.SearchFoodsByNameAsync(name);
            return _mapper.Map<IEnumerable<FoodDto>>(foodEntities);
        }

        public async Task<FoodDto> CreateFoodAsync(FoodCreateDto foodCreateDto)
        {
            var foodEntity = _mapper.Map<FoodEntity>(foodCreateDto);

            await _foodRepository.AddAsync(foodEntity);
            await _foodRepository.SaveAsync();

            var createdFood = await _foodRepository.GetSingleAsync(foodEntity.Id);
            return _mapper.Map<FoodDto>(createdFood);
        }

        public async Task<FoodDto?> UpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto)
        {
            var existingFood = await _foodRepository.GetSingleAsync(id);
            if (existingFood == null)
            {
                return null;
            }

            _mapper.Map(foodUpdateDto, existingFood);
            await _foodRepository.UpdateAsync(id, existingFood);
            await _foodRepository.SaveAsync();

            return _mapper.Map<FoodDto>(existingFood);
        }

        public async Task<FoodDto?> PatchFoodAsync(int id, JsonPatchDocument<FoodUpdateDto> patchDoc)
        {
            var existingFood = await _foodRepository.GetSingleAsync(id);
            if (existingFood == null)
            {
                return null;
            }

            var foodUpdateDto = _mapper.Map<FoodUpdateDto>(existingFood);
            patchDoc.ApplyTo(foodUpdateDto);

            _mapper.Map(foodUpdateDto, existingFood);

            await _foodRepository.UpdateAsync(id, existingFood);
            await _foodRepository.SaveAsync();

            return _mapper.Map<FoodDto>(existingFood);
        }

        public async Task<bool> DeleteFoodAsync(int id)
        {
            var existingFood = await _foodRepository.GetSingleAsync(id);
            if (existingFood == null)
            {
                return false;
            }

            await _foodRepository.DeleteAsync(id);
            await _foodRepository.SaveAsync();

            return true;
        }

        public async Task<IEnumerable<FoodDto>> GetRandomMealAsync()
        {
            var randomMeal = await _foodRepository.GetRandomMealAsync();
            return _mapper.Map<IEnumerable<FoodDto>>(randomMeal);
        }

        public async Task<int> GetTotalFoodCountAsync()
        {
            return await _foodRepository.CountAsync();
        }
    }
}
