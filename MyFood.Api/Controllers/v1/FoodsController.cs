using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Infrastructure.Helpers;
using MyFood.Infrastructure;
using System.Text.Json;

namespace MyFood.Api.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class FoodsController : ControllerBase
    {
        private readonly IFoodService _foodService;
        private readonly ILinkService<FoodsController> _linkService;

        public FoodsController(IFoodService foodService, ILinkService<FoodsController> linkService)
        {
            _foodService = foodService;
            _linkService = linkService;
        }

       
        [HttpGet(Name = nameof(GetAllFoods))]
        public async Task<ActionResult> GetAllFoods(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            var foodDtos = (await _foodService.GetAllFoodsAsync(queryParameters)).ToList();
            var allItemCount = await _foodService.GetTotalFoodCountAsync();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = foodDtos.Select(x => _linkService.ExpandSingleFoodItem(x, x.Id, version));

            return Ok(new { value = toReturn, links });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleFood))]
        public async Task<ActionResult> GetSingleFood(ApiVersion version, int id)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");
            }

            var foodItem = await _foodService.GetFoodByIdAsync(id);

            if (foodItem == null)
            {
                return NotFound();
            }

            return Ok(_linkService.ExpandSingleFoodItem(foodItem, foodItem.Id, version));
        }

        [HttpGet]
        [Route("search", Name = nameof(SearchByName))]
        public async Task<ActionResult> SearchByName(ApiVersion version, [FromQuery] QueryParameters queryParameters, string name)
        {
            var foodItems = (await _foodService.SearchFoodsByNameAsync(name)).ToList();

            var allItemCount = foodItems.Count;
            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = foodItems.Select(x => _linkService.ExpandSingleFoodItem(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpPost(Name = nameof(AddFood))]
        public async Task<ActionResult<FoodDto>> AddFood(ApiVersion version, [FromBody] FoodCreateDto foodCreateDto)
        {
            if (foodCreateDto == null)
            {
                return BadRequest();
            }

            var createdFood = await _foodService.CreateFoodAsync(foodCreateDto);

            return CreatedAtRoute(nameof(GetSingleFood),
                new { version = version.ToString(), id = createdFood.Id },
                _linkService.ExpandSingleFoodItem(createdFood, createdFood.Id, version));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateFood))]
        public async Task<ActionResult<FoodDto>> PartiallyUpdateFood(ApiVersion version, int id, [FromBody] JsonPatchDocument<FoodUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var existingFood = await _foodService.GetFoodByIdAsync(id);

            if (existingFood == null)
            {
                return NotFound();
            }

            var foodUpdateDto = new FoodUpdateDto
            {
                Name = existingFood.Name,
                Type = existingFood.Type,
                Calories = existingFood.Calories,
                Created = existingFood.Created
            };

            patchDoc.ApplyTo(foodUpdateDto, ModelState);

            TryValidateModel(foodUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedFood = await _foodService.UpdateFoodAsync(id, foodUpdateDto);

            if (updatedFood == null)
            {
                return NotFound();
            }

            return Ok(_linkService.ExpandSingleFoodItem(updatedFood, updatedFood.Id, version));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveFood))]
        public async Task<ActionResult> RemoveFood(int id)
        {
            var deleted = await _foodService.DeleteFoodAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateFood))]
        public async Task<ActionResult<FoodDto>> UpdateFood(ApiVersion version, int id, [FromBody] FoodUpdateDto foodUpdateDto)
        {
            if (foodUpdateDto == null)
            {
                return BadRequest();
            }

            var updatedFood = await _foodService.UpdateFoodAsync(id, foodUpdateDto);

            if (updatedFood == null)
            {
                return NotFound();
            }

            return Ok(_linkService.ExpandSingleFoodItem(updatedFood, updatedFood.Id, version));
        }

        [HttpGet("GetRandomMeal", Name = nameof(GetRandomMeal))]
        public async Task<ActionResult> GetRandomMeal()
        {
            var foodItems = await _foodService.GetRandomMealAsync();

            var links = new List<LinkDto>
            {
                new LinkDto(Url.Link(nameof(GetRandomMeal), null), "self", "GET")
            };

            return Ok(new
            {
                value = foodItems,
                links
            });
        }
    }
}
