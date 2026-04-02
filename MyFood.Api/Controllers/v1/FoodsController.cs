using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;  // Add this for IFoodService
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
        private readonly IFoodService _foodService;  // Changed
        private readonly IMapper _mapper;
        private readonly ILinkService<FoodsController> _linkService;

        public FoodsController(
            IFoodService foodService,  // Changed
            IMapper mapper,
            ILinkService<FoodsController> linkService)
        {
            _foodService = foodService;
            _mapper = mapper;
            _linkService = linkService;
        }

        [AllowAnonymous]
        [HttpGet(Name = nameof(GetAllFoods))]
        public async Task<ActionResult> GetAllFoods(  // Made async
            ApiVersion version, 
            [FromQuery] QueryParameters queryParameters)
        {
            // Changed to use service with async
            var foodDtos = await _foodService.GetAllFoodsAsync(queryParameters);
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

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleFood))]
        public async Task<ActionResult> GetSingleFood(  // Made async
            ApiVersion version, 
            int id)
        {
            if (id < 0) throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");

            var foodDto = await _foodService.GetFoodByIdAsync(id);  // Changed
            if (foodDto == null) return NotFound();

            return Ok(_linkService.ExpandSingleFoodItem(foodDto, foodDto.Id, version));
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("search", Name = nameof(SearchByName))]
        public async Task<ActionResult> SearchByName(  // Made async
            ApiVersion version,
            [FromQuery] QueryParameters queryParameters, 
            string name)
        {
            var (foodDtos, totalCount) = await _foodService.SearchFoodsByNameAsync(name, queryParameters);  // Changed

            var paginationMetadata = new
            {
                totalCount = totalCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(totalCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, totalCount, version);
            var toReturn = foodDtos.Select(x => _linkService.ExpandSingleFoodItem(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpPost(Name = nameof(AddFood))]
        public async Task<ActionResult<FoodDto>> AddFood(  // Made async
            ApiVersion version, 
            [FromBody] FoodCreateDto foodCreateDto)
        {
            if (foodCreateDto == null) return BadRequest();

            var createdFood = await _foodService.AddFoodAsync(foodCreateDto);  // Changed

            return CreatedAtRoute(nameof(GetSingleFood),
                new { version = version.ToString(), id = createdFood.Id },
                _linkService.ExpandSingleFoodItem(createdFood, createdFood.Id, version));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateFood))]
        public async Task<ActionResult<FoodDto>> PartiallyUpdateFood(  // Made async
            ApiVersion version, 
            int id, 
            [FromBody] JsonPatchDocument<FoodUpdateDto> patchDoc)
        {
            if (patchDoc == null) return BadRequest();

            var updatedFood = await _foodService.PartiallyUpdateFoodAsync(id, patchDoc);  // Changed
            if (updatedFood == null) return NotFound();

            return Ok(_linkService.ExpandSingleFoodItem(updatedFood, updatedFood.Id, version));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveFood))]
        public async Task<ActionResult> RemoveFood(int id)  // Made async
        {
            var result = await _foodService.DeleteFoodAsync(id);  // Changed
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateFood))]
        public async Task<ActionResult<FoodDto>> UpdateFood(  // Made async
            ApiVersion version, 
            int id, 
            [FromBody] FoodUpdateDto foodUpdateDto)
        {
            if (foodUpdateDto == null) return BadRequest();

            var updatedFood = await _foodService.UpdateFoodAsync(id, foodUpdateDto);  // Changed
            if (updatedFood == null) return NotFound();

            return Ok(_linkService.ExpandSingleFoodItem(updatedFood, updatedFood.Id, version));
        }

        [HttpGet("GetRandomMeal", Name = nameof(GetRandomMeal))]
        public async Task<ActionResult> GetRandomMeal()  // Made async
        {
            var foodDtos = await _foodService.GetRandomMealAsync();  // Changed

            var links = new List<LinkDto> 
            { 
                new LinkDto(Url.Link(nameof(GetRandomMeal), null), "self", "GET") 
            };

            return Ok(new { value = foodDtos, links = links });
        }
    }
}
