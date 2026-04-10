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
            var foodEntities = await _foodRepository.GetAllAsync(queryParameters);
            return _mapper.Map<IEnumerable<FoodDto>>(foodEntities);
        }

        public async Task<int> GetTotalFoodCountAsync()
        {
            return await _foodRepository.CountAsync();
        }

        public async Task<FoodDto?> GetFoodByIdAsync(int id)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");
            }

            var foodEntity = await _foodRepository.GetSingleAsync(id);
            return foodEntity is null ? null : _mapper.Map<FoodDto>(foodEntity);
        }

        public async Task<IEnumerable<FoodDto>> SearchFoodsByNameAsync(string name)
        {
            var foodEntities = await _foodRepository.SearchFoodsByNameAsync(name);
            return _mapper.Map<IEnumerable<FoodDto>>(foodEntities);
        }

        public async Task<FoodDto> CreateFoodAsync(FoodCreateDto foodCreateDto)
        {
            ArgumentNullException.ThrowIfNull(foodCreateDto);

            var entity = _mapper.Map<FoodEntity>(foodCreateDto);
            await _foodRepository.AddAsync(entity);

            if (!await _foodRepository.SaveAsync())
            {
                throw new Exception("Creating a food item failed on save.");
            }

            var savedEntity = await _foodRepository.GetSingleAsync(entity.Id)
                ?? throw new Exception("Food item was created but could not be reloaded.");

            return _mapper.Map<FoodDto>(savedEntity);
        }

        public async Task<FoodDto?> UpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto)
        {
            ArgumentNullException.ThrowIfNull(foodUpdateDto);

            var existingFoodItem = await _foodRepository.GetSingleAsync(id);
            if (existingFoodItem is null)
            {
                return null;
            }

            _mapper.Map(foodUpdateDto, existingFoodItem);
            _foodRepository.Update(existingFoodItem);

            if (!await _foodRepository.SaveAsync())
            {
                throw new Exception("Updating a food item failed on save.");
            }

            return _mapper.Map<FoodDto>(existingFoodItem);
        }

        public async Task<FoodDto?> PartiallyUpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto)
        {
            return await UpdateFoodAsync(id, foodUpdateDto);
        }

        public async Task<bool> DeleteFoodAsync(int id)
        {
            var existingFoodItem = await _foodRepository.GetSingleAsync(id);
            if (existingFoodItem is null)
            {
                return false;
            }

            _foodRepository.Delete(existingFoodItem);

            if (!await _foodRepository.SaveAsync())
            {
                throw new Exception("Deleting a food item failed on save.");
            }

            return true;
        }

        public async Task<IEnumerable<FoodDto>> GetRandomMealAsync()
        {
            var foodEntities = await _foodRepository.GetRandomMealAsync();
            return _mapper.Map<IEnumerable<FoodDto>>(foodEntities);
        }

        public async Task<FoodUpdateDto?> GetFoodForPatchAsync(int id)
        {
            var foodEntity = await _foodRepository.GetSingleAsync(id);
            return foodEntity is null ? null : _mapper.Map<FoodUpdateDto>(foodEntity);
        }
    }
}