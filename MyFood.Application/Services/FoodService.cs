using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services;
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
        var foodEntity = _mapper.Map<FoodEntity>(foodCreateDto);

        _foodRepository.Add(foodEntity);
        _foodRepository.Save();

        return Task.FromResult(_mapper.Map<FoodDto>(foodEntity));
    }

    public Task<bool> DeleteFoodAsync(int id)
    {
        var foodItem = _foodRepository.GetSingle(id);

        if (foodItem == null)
        {
            return Task.FromResult(false);
        }

        _foodRepository.Delete(id);

        return Task.FromResult(_foodRepository.Save());
    }

    public Task<IEnumerable<FoodDto>> GetAllFoodsAsync(QueryParameters queryParameters)
    {
        var foodEntities = _foodRepository.GetAll(queryParameters);

        return Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foodEntities));
    }

    public Task<FoodDto?> GetFoodByIdAsync(int id)
    {
        var foodEntity = _foodRepository.GetSingle(id);
        return Task.FromResult(foodEntity != null ? _mapper.Map<FoodDto>(foodEntity) : null);
    }

    public Task<IEnumerable<FoodDto>> GetRandomMealAsync()
    {
        var randomMealEntities = _foodRepository.GetRandomMeal();
        return Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(randomMealEntities));
    }

    public Task<int> GetTotalFoodCountAsync()
    {
        var count = _foodRepository.Count();
        return Task.FromResult(count);
    }

    public Task<IEnumerable<FoodDto>> SearchFoodsByNameAsync(string name)
    {
        var foodEntities = _foodRepository.SearchFoodsByName(name);
        return Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foodEntities));
    }

    public Task<FoodDto?> UpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto)
    {
        var existingFoodEntity = _foodRepository.GetSingle(id);

        if (existingFoodEntity == null)
        {
            return Task.FromResult<FoodDto?>(null);
        }
        _mapper.Map(foodUpdateDto, existingFoodEntity);
        _foodRepository.Update(id, existingFoodEntity);
        _foodRepository.Save();

        return Task.FromResult(_mapper.Map<FoodDto?>(existingFoodEntity));
    }
}