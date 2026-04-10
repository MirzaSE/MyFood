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
        var entities = _foodRepository.GetAll(queryParameters);
        return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(entities));
    }

    public async Task<int> GetTotalFoodCountAsync()
    {
        return await Task.FromResult(_foodRepository.Count());
    }

    public async Task<FoodDto?> GetFoodByIdAsync(int id)
    {
        var entity = _foodRepository.GetSingle(id);
        if (entity == null) return null;
        return await Task.FromResult(_mapper.Map<FoodDto>(entity));
    }

    public async Task<IEnumerable<FoodDto>> SearchFoodsByNameAsync(string name)
    {
        var entities = _foodRepository.SearchFoodsByName(name);
        return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(entities));
    }

    public async Task<FoodDto> CreateFoodAsync(FoodCreateDto foodCreateDto)
    {
        var entity = _mapper.Map<FoodEntity>(foodCreateDto);
        _foodRepository.Add(entity);
        if (!_foodRepository.Save())
            throw new Exception("Creating a food item failed on save.");
        var newEntity = _foodRepository.GetSingle(entity.Id);
        return await Task.FromResult(_mapper.Map<FoodDto>(newEntity));
    }

    public async Task<FoodUpdateDto?> GetFoodForUpdateAsync(int id)
    {
        var entity = _foodRepository.GetSingle(id);
        if (entity == null) return null;
        return await Task.FromResult(_mapper.Map<FoodUpdateDto>(entity));
    }

    public async Task<FoodDto?> UpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto)
    {
        var existingEntity = _foodRepository.GetSingle(id);
        if (existingEntity == null) return null;
        _mapper.Map(foodUpdateDto, existingEntity);
        var updated = _foodRepository.Update(id, existingEntity);
        if (!_foodRepository.Save())
            throw new Exception("Updating a food item failed on save.");
        return await Task.FromResult(_mapper.Map<FoodDto>(updated));
    }

    public async Task<bool> DeleteFoodAsync(int id)
    {
        var entity = _foodRepository.GetSingle(id);
        if (entity == null) return false;
        _foodRepository.Delete(id);
        if (!_foodRepository.Save())
            throw new Exception("Deleting a food item failed on save.");
        return await Task.FromResult(true);
    }

    public async Task<IEnumerable<FoodDto>> GetRandomMealAsync()
    {
        var entities = _foodRepository.GetRandomMeal();
        return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(entities));
    }
}
