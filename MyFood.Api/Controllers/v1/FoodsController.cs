using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class FoodsController : ControllerBase
{
    private readonly IFoodService _foodService;

    public FoodsController(IFoodService foodService)
    {
        _foodService = foodService;
    }

    [HttpGet(Name = nameof(GetAllFoods))]
    public ActionResult GetAllFoods(ApiVersion version, [FromQuery] QueryParameters queryParameters)
    {
        var (foods, pagination, links) = _foodService.GetAllFoods(queryParameters, version);
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
        return Ok(new { value = foods, links });
    }

    [HttpGet("{id:int}", Name = nameof(GetSingleFood))]
    public ActionResult GetSingleFood(ApiVersion version, int id)
    {
        var (food, notFound) = _foodService.GetSingleFood(id, version);
        if (notFound) return NotFound();
        return Ok(food);
    }

    [HttpGet("search", Name = nameof(SearchByName))]
    public ActionResult SearchByName(ApiVersion version, [FromQuery] QueryParameters queryParameters, string name)
    {
        var (foods, pagination, links) = _foodService.SearchFoodsByName(name, queryParameters, version);
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
        return Ok(new { value = foods, links });
    }

    [HttpPost(Name = nameof(AddFood))]
    public ActionResult<FoodDto> AddFood(ApiVersion version, [FromBody] FoodCreateDto foodCreateDto)
    {
        var (food, created, error) = _foodService.AddFood(foodCreateDto, version);
        if (!created) return BadRequest(error);
        string json = JsonSerializer.Serialize(food);
        FoodDto foodDto = JsonSerializer.Deserialize<FoodDto>(json);
        return CreatedAtRoute(nameof(GetSingleFood), new { version = version.ToString(), id = foodDto?.Id }, food);
    }

    [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateFood))]
    public ActionResult<FoodDto> PartiallyUpdateFood(ApiVersion version, int id, [FromBody] JsonPatchDocument<FoodUpdateDto> patchDoc)
    {
        var (food, notFound, error) = _foodService.PartiallyUpdateFood(id, patchDoc, version, ModelState);
        if (notFound) return NotFound();
        if (error != null) return BadRequest(error);
        return Ok(food);
    }

    [HttpDelete("{id:int}", Name = nameof(RemoveFood))]
    public ActionResult RemoveFood(int id)
    {
        var (notFound, error) = _foodService.RemoveFood(id);
        if (notFound) return NotFound();
        if (error != null) return BadRequest(error);
        return NoContent();
    }

    [HttpPut("{id:int}", Name = nameof(UpdateFood))]
    public ActionResult<FoodDto> UpdateFood(ApiVersion version, int id, [FromBody] FoodUpdateDto foodUpdateDto)
    {
        var (food, notFound, error) = _foodService.UpdateFood(id, foodUpdateDto, version);
        if (notFound) return NotFound();
        if (error != null) return BadRequest(error);
        return Ok(food);
    }

    [HttpGet("GetRandomMeal", Name = nameof(GetRandomMeal))]
    public ActionResult GetRandomMeal(ApiVersion version)
    {
        var (foods, links) = _foodService.GetRandomMeal(version, Url.Link);
        return Ok(new { value = foods, links });
    }
}