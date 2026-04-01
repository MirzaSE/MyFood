using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Helpers;
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

        public FoodsController(
            IFoodService foodService,
            ILinkService<FoodsController> linkService)
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

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = foodDtos.Select(x => _linkService.ExpandSingleFoodItem(x, x.Id, version));

            return Ok(new { value = toReturn, links });
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleFood))]
        public async Task<ActionResult> GetSingleFood(ApiVersion version, int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");

            var item = await _foodService.GetFoodByIdAsync(id);
            if (item == null) return NotFound();

            return Ok(_linkService.ExpandSingleFoodItem(item, item.Id, version));
        }

        [HttpGet("search", Name = nameof(SearchByName))]
        public async Task<ActionResult> SearchByName(
            ApiVersion version,
            [FromQuery] QueryParameters queryParameters,
            [FromQuery] string name)
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

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = foodItems.Select(x => _linkService.ExpandSingleFoodItem(x, x.Id, version));

            return Ok(new { value = toReturn, links });
        }

        [HttpPost(Name = nameof(AddFood))]
        public async Task<ActionResult> AddFood(ApiVersion version, [FromBody] FoodCreateDto foodCreateDto)
        {
            if (foodCreateDto == null) return BadRequest();

            var created = await _foodService.CreateFoodAsync(foodCreateDto);

            return CreatedAtRoute(
                nameof(GetSingleFood),
                new { version = version.ToString(), id = created.Id },
                _linkService.ExpandSingleFoodItem(created, created.Id, version));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateFood))]
        public async Task<ActionResult> PartiallyUpdateFood(
            ApiVersion version,
            int id,
            [FromBody] JsonPatchDocument<FoodUpdateDto> patchDoc)
        {
            if (patchDoc == null) return BadRequest();

            var existing = await _foodService.GetFoodByIdAsync(id);
            if (existing == null) return NotFound();

            var foodUpdateDto = new FoodUpdateDto
            {
                Name = existing.Name,
                Type = existing.Type,
                Calories = existing.Calories
            };

            patchDoc.ApplyTo(foodUpdateDto, ModelState);

            if (!TryValidateModel(foodUpdateDto))
                return BadRequest(ModelState);

            var updated = await _foodService.PatchFoodAsync(id, foodUpdateDto);
            if (updated == null) return NotFound();

            return Ok(_linkService.ExpandSingleFoodItem(updated, updated.Id, version));
        }

        [HttpDelete("{id:int}", Name = nameof(RemoveFood))]
        public async Task<ActionResult> RemoveFood(int id)
        {
            var deleted = await _foodService.DeleteFoodAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }

        [HttpPut("{id:int}", Name = nameof(UpdateFood))]
        public async Task<ActionResult> UpdateFood(
            ApiVersion version,
            int id,
            [FromBody] FoodUpdateDto foodUpdateDto)
        {
            if (foodUpdateDto == null) return BadRequest();

            var updated = await _foodService.UpdateFoodAsync(id, foodUpdateDto);
            if (updated == null) return NotFound();

            return Ok(_linkService.ExpandSingleFoodItem(updated, updated.Id, version));
        }

        [HttpGet("GetRandomMeal", Name = nameof(GetRandomMeal))]
        public async Task<ActionResult> GetRandomMeal()
        {
            var foodItems = (await _foodService.GetRandomMealAsync()).ToList();

            var links = new List<LinkDto>
            {
                new LinkDto(Url.Link(nameof(GetRandomMeal), null), "self", "GET")
            };

            return Ok(new { value = foodItems, links });
        }
    }
}