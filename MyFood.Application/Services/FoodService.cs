using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Helpers;
using MyFood.Domain.Entities;
using MyFood.Domain.Interfaces;
using MyFood.Infrastructure;

public class FoodService : IFoodService
{
    private readonly IFoodRepository _foodRepository;
    private readonly IMapper _mapper;
    private readonly ILinkService<FoodService> _linkService;

    public FoodService(
        IFoodRepository foodRepository,
        IMapper mapper,
        ILinkService<FoodService> linkService)
    {
        _foodRepository = foodRepository;
        _mapper = mapper;
        _linkService = linkService;
    }

    public (IEnumerable<object> Foods, object Pagination, List<LinkDto> Links) GetAllFoods(QueryParameters queryParameters, ApiVersion version)
    {
        var foodItems = _foodRepository.GetAll(queryParameters.Query, queryParameters.PageCount, queryParameters.Page).ToList();
        var allItemCount = _foodRepository.Count();

        var paginationMetadata = new
        {
            totalCount = allItemCount,
            pageSize = queryParameters.PageCount,
            currentPage = queryParameters.Page,
            totalPages = queryParameters.GetTotalPages(allItemCount)
        };

        var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
        var toReturn = foodItems.Select(x => _linkService.ExpandSingleFoodItem(_mapper.Map<FoodDto>(x), x.Id, version));

        return (toReturn, paginationMetadata, links);
    }

    public (object Food, bool NotFound) GetSingleFood(int id, ApiVersion version)
    {
        if (id < 0)
            return (null, true);

        var foodItem = _foodRepository.GetSingle(id);
        if (foodItem == null)
            return (null, true);

        var foodDto = _mapper.Map<FoodDto>(foodItem);
        return (_linkService.ExpandSingleFoodItem(foodDto, foodDto.Id, version), false);
    }

    public (IEnumerable<object> Foods, object Pagination, List<LinkDto> Links) SearchFoodsByName(string name, QueryParameters queryParameters, ApiVersion version)
    {
        var foodItems = _foodRepository.SearchFoodsByName(name);
        var allItemCount = foodItems.Count();

        var paginationMetadata = new
        {
            totalCount = allItemCount,
            pageSize = queryParameters.PageCount,
            currentPage = queryParameters.Page,
            totalPages = queryParameters.GetTotalPages(allItemCount)
        };

        var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
        var toReturn = foodItems.Select(x => _linkService.ExpandSingleFoodItem(_mapper.Map<FoodDto>(x), x.Id, version));

        return (toReturn, paginationMetadata, links);
    }

    public (object Food, bool Created, string? Error) AddFood(FoodCreateDto foodCreateDto, ApiVersion version)
    {
        if (foodCreateDto == null)
            return (null, false, "Invalid input");

        var toAdd = _mapper.Map<FoodEntity>(foodCreateDto);
        _foodRepository.Add(toAdd);

        if (!_foodRepository.Save())
            return (null, false, "Creating a fooditem failed on save.");

        var newFoodItem = _foodRepository.GetSingle(toAdd.Id);
        var foodDto = _mapper.Map<FoodDto>(newFoodItem);

        return (_linkService.ExpandSingleFoodItem(foodDto, foodDto.Id, version), true, null);
    }

    public (object Food, bool NotFound, string? Error) PartiallyUpdateFood(int id, JsonPatchDocument<FoodUpdateDto> patchDoc, ApiVersion version, ModelStateDictionary modelState)
    {
        if (patchDoc == null)
            return (null, false, "Invalid patch document");

        var existingEntity = _foodRepository.GetSingle(id);
        if (existingEntity == null)
            return (null, true, null);

        var foodUpdateDto = _mapper.Map<FoodUpdateDto>(existingEntity);
        patchDoc.ApplyTo(foodUpdateDto);

        //tryValidateModel(foodUpdateDto);

        if (!modelState.IsValid)
            return (null, false, "Invalid model state");

        _mapper.Map(foodUpdateDto, existingEntity);
        var updated = _foodRepository.Update(id, existingEntity);

        if (!_foodRepository.Save())
            return (null, false, "Updating a fooditem failed on save.");

        var foodDto = _mapper.Map<FoodDto>(updated);
        return (_linkService.ExpandSingleFoodItem(foodDto, foodDto.Id, version), false, null);
    }

    public (object Food, bool NotFound, string? Error) UpdateFood(int id, FoodUpdateDto foodUpdateDto, ApiVersion version)
    {
        if (foodUpdateDto == null)
            return (null, false, "Invalid input");

        var existingFoodItem = _foodRepository.GetSingle(id);
        if (existingFoodItem == null)
            return (null, true, null);

        _mapper.Map(foodUpdateDto, existingFoodItem);
        _foodRepository.Update(id, existingFoodItem);

        if (!_foodRepository.Save())
            return (null, false, "Updating a fooditem failed on save.");

        var foodDto = _mapper.Map<FoodDto>(existingFoodItem);
        return (_linkService.ExpandSingleFoodItem(foodDto, foodDto.Id, version), false, null);
    }

    public (bool NotFound, string? Error) RemoveFood(int id)
    {
        var foodItem = _foodRepository.GetSingle(id);
        if (foodItem == null)
            return (true, null);

        _foodRepository.Delete(id);

        if (!_foodRepository.Save())
            return (false, "Deleting a fooditem failed on save.");

        return (false, null);
    }

    public (IEnumerable<object> Foods, List<LinkDto> Links) GetRandomMeal(ApiVersion version, Func<string, object?, string?> urlLink)
    {
        var foodItems = _foodRepository.GetRandomMeal();
        var dtos = foodItems.Select(x => _mapper.Map<FoodDto>(x));
        var links = new List<LinkDto>
        {
            new LinkDto(urlLink(nameof(GetRandomMeal), null), "self", "GET")
        };
        var expanded = dtos.Select(x => _linkService.ExpandSingleFoodItem(x, x.Id, version));
        return (expanded, links);
    }
}