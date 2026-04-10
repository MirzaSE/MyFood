using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
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
        FoodEntity toAdd = _mapper.Map<FoodEntity>(foodCreateDto);
        _foodRepository.Add(toAdd);
        if (!_foodRepository.Save())
        {
            throw new Exception("Creating a fooditem failed on save.");
        }
        FoodEntity newFoodItem = _foodRepository.GetSingle(toAdd.Id);
        FoodDto foodDto = _mapper.Map<FoodDto>(newFoodItem);
        return Task.FromResult(foodDto);
    }

    public Task<bool> DeleteFoodAsync(int id)
    {
        FoodEntity foodItem = _foodRepository.GetSingle(id);
        if (foodItem == null)
        {
            return Task.FromResult(false);
        }
        _foodRepository.Delete(id);
        if (!_foodRepository.Save())
        {
            throw new Exception("Deleting a fooditem failed on save.");
        }
        return Task.FromResult(true);
    }

    public async Task<IEnumerable<FoodDto>> GetAllFoodsAsync(QueryParameters queryParameters)
    {
        var foodEntities = _foodRepository.GetAll(queryParameters);
        return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foodEntities));
    }

    public async Task<FoodDto?> GetFoodByIdAsync(int id)
    {
        FoodEntity foodItem = await _foodRepository.GetSingleAsync(id);
        return await Task.FromResult(_mapper.Map<FoodDto?>(foodItem));
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

    public async Task<IEnumerable<FoodDto>> SearchFoodsByNameAsync(string name)
    {
        var foodEntities = _foodRepository.SearchFoodsByName(name);
        return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foodEntities));
    }

    public Task<FoodDto?> UpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto)
    {
        var existingFoodItem = _foodRepository.GetSingle(id);
        if (existingFoodItem == null)
        {
            return Task.FromResult<FoodDto?>(null);
        }
        _mapper.Map(foodUpdateDto, existingFoodItem);

        _foodRepository.Update(id, existingFoodItem);

        if (!_foodRepository.Save())
        {
            throw new Exception("Updating a fooditem failed on save.");
        }

        FoodDto foodDto = _mapper.Map<FoodDto>(existingFoodItem);
        return Task.FromResult<FoodDto?>(foodDto);
    }

    public Task<FoodDto?> PartialUpdateFoodAsync(int id, JsonPatchDocument<FoodUpdateDto> patchDoc)
    {
        var foodEntity = _foodRepository.GetSingle(id);

        FoodUpdateDto foodUpdateDto = _mapper.Map<FoodUpdateDto>(foodEntity);
        patchDoc.ApplyTo(foodUpdateDto);

        _mapper.Map(foodUpdateDto, foodEntity);

        var updatedFood = _foodRepository.Update(id, foodEntity);

        return Task.FromResult(_mapper.Map<FoodDto?>(updatedFood));
    }
}
