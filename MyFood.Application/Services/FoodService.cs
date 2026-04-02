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

        public Task<IEnumerable<FoodDto>> GetAllFoodsAsync(QueryParameters queryParameters)
        {
            var foodEntities = _foodRepository.GetAll(queryParameters).ToList();
            var dtos = _mapper.Map<IEnumerable<FoodDto>>(foodEntities);
            return Task.FromResult(dtos);
        }

        public Task<FoodDto?> GetFoodByIdAsync(int id)
        {
            var foodEntity = _foodRepository.GetSingle(id);
            if (foodEntity == null)
            {
                return Task.FromResult<FoodDto?>(null);
            }

            var dto = _mapper.Map<FoodDto>(foodEntity);
            return Task.FromResult<FoodDto?>(dto);
        }

        public Task<IEnumerable<FoodDto>> SearchFoodsByNameAsync(string name)
        {
            var foodEntities = _foodRepository.SearchFoodsByName(name).ToList();
            var dtos = _mapper.Map<IEnumerable<FoodDto>>(foodEntities);
            return Task.FromResult(dtos);
        }

        public Task<FoodDto> CreateFoodAsync(FoodCreateDto foodCreateDto)
        {
            var entity = _mapper.Map<FoodEntity>(foodCreateDto);
            _foodRepository.Add(entity);

            if (!_foodRepository.Save())
            {
                throw new InvalidOperationException("Creating a fooditem failed on save.");
            }

            var createdEntity = _foodRepository.GetSingle(entity.Id) ?? entity;
            var dto = _mapper.Map<FoodDto>(createdEntity);
            return Task.FromResult(dto);
        }

        public Task<FoodDto?> UpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto)
        {
            var existingEntity = _foodRepository.GetSingle(id);
            if (existingEntity == null)
            {
                return Task.FromResult<FoodDto?>(null);
            }

            _mapper.Map(foodUpdateDto, existingEntity);
            var updatedEntity = _foodRepository.Update(id, existingEntity);

            if (!_foodRepository.Save())
            {
                throw new InvalidOperationException("Updating a fooditem failed on save.");
            }

            var dto = _mapper.Map<FoodDto>(updatedEntity);
            return Task.FromResult<FoodDto?>(dto);
        }

        public Task<bool> DeleteFoodAsync(int id)
        {
            var existingEntity = _foodRepository.GetSingle(id);
            if (existingEntity == null)
            {
                return Task.FromResult(false);
            }

            _foodRepository.Delete(id);

            if (!_foodRepository.Save())
            {
                throw new InvalidOperationException("Deleting a fooditem failed on save.");
            }

            return Task.FromResult(true);
        }

        public Task<IEnumerable<FoodDto>> GetRandomMealAsync()
        {
            var foodItems = _foodRepository.GetRandomMeal();
            var dtos = _mapper.Map<IEnumerable<FoodDto>>(foodItems);
            return Task.FromResult(dtos);
        }

        public Task<int> GetTotalFoodCountAsync()
        {
            return Task.FromResult(_foodRepository.Count());
        }
    }
}