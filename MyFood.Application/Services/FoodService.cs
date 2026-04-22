using MyFood.Application.Dtos;
using MyFood.Application;
using MyFood.Application.Services;
using AutoMapper;
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

        public Task<FoodDto> CreateFoodAsync(FoodCreateDto foodCreateDto)
        {
            var entity = _mapper.Map<FoodEntity>(foodCreateDto);

            _foodRepository.Add(entity);
            var saved = _foodRepository.Save();
            if (!saved)
            {
                throw new InvalidOperationException("Creating a food item failed on save.");
            }

            var created = _foodRepository.GetSingle(entity.Id);
            var dto = _mapper.Map<FoodDto>(created);
            return Task.FromResult(dto);
        }

        public Task<bool> DeleteFoodAsync(int id)
        {
            var existing = _foodRepository.GetSingle(id);
            if (existing == null)
            {
                return Task.FromResult(false);
            }

            _foodRepository.Delete(id);
            var saved = _foodRepository.Save();
            return Task.FromResult(saved);
        }

        public async Task<IEnumerable<FoodDto>> GetAllFoodsAsync(QueryParameters queryParameters)
        {
            var foodEntities = _foodRepository.GetAll(queryParameters);
            return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foodEntities));
        }

        public Task<FoodDto?> GetFoodByIdAsync(int id)
        {
            var entity = _foodRepository.GetSingle(id);
            if (entity == null)
            {
                return Task.FromResult<FoodDto?>(null);
            }

            var dto = _mapper.Map<FoodDto>(entity);
            return Task.FromResult<FoodDto?>(dto);
        }

        public Task<IEnumerable<FoodDto>> GetRandomMealAsync()
        {
            var entities = _foodRepository.GetRandomMeal();
            var dtos = _mapper.Map<IEnumerable<FoodDto>>(entities);
            return Task.FromResult(dtos);
        }

        public Task<int> GetTotalFoodCountAsync()
        {
            return Task.FromResult(_foodRepository.Count());
        }

        public Task<IEnumerable<FoodDto>> SearchFoodsByNameAsync(string name)
        {
            var entities = _foodRepository.SearchFoodsByName(name);
            var dtos = _mapper.Map<IEnumerable<FoodDto>>(entities);
            return Task.FromResult(dtos);
        }

        public Task<FoodDto?> UpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto)
        {
            var existing = _foodRepository.GetSingle(id);
            if (existing == null)
            {
                return Task.FromResult<FoodDto?>(null);
            }

            _mapper.Map(foodUpdateDto, existing);

            var updated = _foodRepository.Update(id, existing);
            var saved = _foodRepository.Save();
            if (!saved)
            {
                throw new InvalidOperationException("Updating a food item failed on save.");
            }

            var dto = _mapper.Map<FoodDto>(updated);
            return Task.FromResult<FoodDto?>(dto);
        }
    }
}
