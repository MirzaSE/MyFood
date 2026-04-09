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
            var foodEntity = _foodRepository.GetSingle(id);
            if (foodEntity == null)
            {
                return null;
            }

            return await Task.FromResult(_mapper.Map<FoodDto>(foodEntity));
        }

        public async Task<IEnumerable<FoodDto>> SearchFoodsByNameAsync(string name)
        {
            var foodEntities = _foodRepository.SearchFoodsByName(name).ToList();
            return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foodEntities));
        }

        public async Task<FoodDto> CreateFoodAsync(FoodCreateDto foodCreateDto)
        {
            var entity = _mapper.Map<FoodEntity>(foodCreateDto);
            _foodRepository.Add(entity);

            if (!_foodRepository.Save())
            {
                throw new InvalidOperationException("Creating a fooditem failed on save.");
            }

            var createdEntity = _foodRepository.GetSingle(entity.Id) ?? entity;
            return await Task.FromResult(_mapper.Map<FoodDto>(createdEntity));
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
                throw new InvalidOperationException("Updating a fooditem failed on save.");
            }

            return await Task.FromResult(_mapper.Map<FoodDto>(updatedEntity));
        }

        public async Task<FoodDto?> PatchFoodAsync(int id, FoodUpdateDto foodUpdateDto)
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
                throw new InvalidOperationException("Updating a fooditem failed on save.");
            }

            return await Task.FromResult(_mapper.Map<FoodDto>(updatedEntity));
        }

        public async Task<bool> DeleteFoodAsync(int id)
        {
            var existingEntity = _foodRepository.GetSingle(id);
            if (existingEntity == null)
            {
                return false;
            }

            _foodRepository.Delete(id);

            if (!_foodRepository.Save())
            {
                throw new InvalidOperationException("Deleting a fooditem failed on save.");
            }

            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<FoodDto>> GetRandomMealAsync()
        {
            var foodItems = _foodRepository.GetRandomMeal();
            return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foodItems));
        }

        public Task<int> GetTotalFoodCountAsync()
        {
            return Task.FromResult(_foodRepository.Count());
        }
    }
}
