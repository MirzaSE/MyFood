using AutoMapper;
using MyFood.Application.Dtos;
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
            var foodEntities = _foodRepository.GetAll(queryParameters).ToList();
            return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foodEntities));
        }

        public async Task<FoodDto?> GetFoodByIdAsync(int id)
        {
            var item = _foodRepository.GetSingle(id);
            if (item == null) return null;

            return await Task.FromResult(_mapper.Map<FoodDto>(item));
        }

        public async Task<IEnumerable<FoodDto>> SearchFoodsByNameAsync(string name)
        {
            var foodItems = _foodRepository.SearchFoodsByName(name).ToList();
            return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foodItems));
        }

        public async Task<FoodDto> CreateFoodAsync(FoodCreateDto foodCreateDto)
        {
            var entity = _mapper.Map<FoodEntity>(foodCreateDto);
            _foodRepository.Add(entity);

            if (!_foodRepository.Save())
                throw new Exception("Creating a fooditem failed on save.");

            var newFoodItem = _foodRepository.GetSingle(entity.Id)!;
            return await Task.FromResult(_mapper.Map<FoodDto>(newFoodItem));
        }

        public async Task<FoodDto?> UpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto)
        {
            var existingFoodItem = _foodRepository.GetSingle(id);
            if (existingFoodItem == null) return null;

            _mapper.Map(foodUpdateDto, existingFoodItem);
            var updated = _foodRepository.Update(id, existingFoodItem);

            if (!_foodRepository.Save())
                throw new Exception("Updating a fooditem failed on save.");

            return await Task.FromResult(_mapper.Map<FoodDto>(updated));
        }

        public async Task<FoodDto?> PatchFoodAsync(int id, FoodUpdateDto foodUpdateDto)
        {
            var existingEntity = _foodRepository.GetSingle(id);
            if (existingEntity == null) return null;

            _mapper.Map(foodUpdateDto, existingEntity);
            var updated = _foodRepository.Update(id, existingEntity);

            if (!_foodRepository.Save())
                throw new Exception("Updating a fooditem failed on save.");

            return await Task.FromResult(_mapper.Map<FoodDto>(updated));
        }

        public async Task<bool> DeleteFoodAsync(int id)
        {
            var foodItem = _foodRepository.GetSingle(id);
            if (foodItem == null) return false;

            _foodRepository.Delete(id);

            if (!_foodRepository.Save())
                throw new Exception("Deleting a fooditem failed on save.");

            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<FoodDto>> GetRandomMealAsync()
        {
            var foodItems = _foodRepository.GetRandomMeal();
            return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foodItems));
        }

        public async Task<int> GetTotalFoodCountAsync()
        {
            return await Task.FromResult(_foodRepository.Count());
        }
    }
}