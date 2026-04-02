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

    public async Task<IEnumerable<FoodDto>> GetAllFoodsAsync(QueryParameters queryParameters)
    {
        var foods = _foodRepository.GetAll(queryParameters);
        return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foods));
    }

    public async Task<FoodDto?> GetFoodByIdAsync(int id)
    {
        var food = _foodRepository.GetSingle(id);
        if (food == null)
        {
            return null;
        }

        return await Task.FromResult(_mapper.Map<FoodDto>(food));
    }

    public async Task<IEnumerable<FoodDto>> SearchFoodsByNameAsync(string name)
    {
        var foods = _foodRepository.SearchFoodsByName(name);
        return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foods));
    }

    public async Task<FoodDto?> CreateFoodAsync(FoodCreateDto foodCreateDto)
    {
        var foodEntity = _mapper.Map<FoodEntity>(foodCreateDto);
        _foodRepository.Add(foodEntity);

        if (!_foodRepository.Save())
        {
            return null;
        }

        var created = _foodRepository.GetSingle(foodEntity.Id);
        return created == null ? null : await Task.FromResult(_mapper.Map<FoodDto>(created));
    }

    public async Task<FoodDto?> UpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto)
    {
        var existing = _foodRepository.GetSingle(id);
        if (existing == null)
        {
            return null;
        }

        _mapper.Map(foodUpdateDto, existing);
        var updated = _foodRepository.Update(id, existing);

        if (!_foodRepository.Save())
        {
            return null;
        }

        return await Task.FromResult(_mapper.Map<FoodDto>(updated));
    }

    public async Task<FoodDto?> PartialUpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto)
    {
        var existing = _foodRepository.GetSingle(id);
        if (existing == null)
        {
            return null;
        }

        _mapper.Map(foodUpdateDto, existing);
        var updated = _foodRepository.Update(id, existing);

        if (!_foodRepository.Save())
        {
            return null;
        }

        return await Task.FromResult(_mapper.Map<FoodDto>(updated));
    }

    public async Task<bool> DeleteFoodAsync(int id)
    {
        var existing = _foodRepository.GetSingle(id);
        if (existing == null)
        {
            return await Task.FromResult(false);
        }

        _foodRepository.Delete(id);
        return await Task.FromResult(_foodRepository.Save());
    }

    public async Task<IEnumerable<FoodDto>> GetRandomMealAsync()
    {
        var foods = _foodRepository.GetRandomMeal();
        return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foods));
    }

    public async Task<int> GetTotalFoodCountAsync()
    {
        return await Task.FromResult(_foodRepository.Count());
    }
}
