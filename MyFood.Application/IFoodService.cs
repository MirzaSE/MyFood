using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public interface IFoodService
{
    (IEnumerable<object> Foods, object Pagination, List<LinkDto> Links) GetAllFoods(QueryParameters queryParameters, ApiVersion version);
    (object Food, bool NotFound) GetSingleFood(int id, ApiVersion version);
    (IEnumerable<object> Foods, object Pagination, List<LinkDto> Links) SearchFoodsByName(string name, QueryParameters queryParameters, ApiVersion version);
    (object Food, bool Created, string? Error) AddFood(FoodCreateDto foodCreateDto, ApiVersion version);
    (object Food, bool NotFound, string? Error) PartiallyUpdateFood(int id, JsonPatchDocument<FoodUpdateDto> patchDoc, ApiVersion version, ModelStateDictionary modelState);
    (object Food, bool NotFound, string? Error) UpdateFood(int id, FoodUpdateDto foodUpdateDto, ApiVersion version);
    (bool NotFound, string? Error) RemoveFood(int id);
    (IEnumerable<object> Foods, List<LinkDto> Links) GetRandomMeal(ApiVersion version, Func<string, object?, string?> urlLink);
}