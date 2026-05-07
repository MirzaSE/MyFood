using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
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
        private readonly IMapper _mapper;
        private readonly ILinkService<FoodsController> _linkService;

        public FoodsController(
            IFoodService foodService,
            IMapper mapper,
            ILinkService<FoodsController> linkService)
        {
            _foodService = foodService;
            _mapper = mapper;
            _linkService = linkService;
        }

       
        [HttpGet(Name = nameof(GetAllFoods))]
        public async Task<ActionResult> GetAllFoods(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            var foodDtos = await _foodService.GetAllFoodsAsync(queryParameters);
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

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleFood))]
        public async Task<ActionResult> GetSingleFood(ApiVersion version, int id)
        {

            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");
            }

            var foodDto = await _foodService.GetFoodByIdAsync(id);

            if (foodDto == null)
            {
                return NotFound();
            }

            return Ok(_linkService.ExpandSingleFoodItem(foodDto, foodDto.Id, version));
        }

        [HttpGet]
        [Route("search", Name = nameof(SearchByName))]
        public async Task<ActionResult> SearchByName(ApiVersion version,[FromQuery] QueryParameters queryParameters, string name)
        {
            var foodDtos = await _foodService.SearchFoodsByNameAsync(name);

            var allItemCount = foodDtos.Count();
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

            var foodDto = await _foodService.CreateFoodAsync(foodCreateDto);

            return CreatedAtRoute(nameof(GetSingleFood),
                new { version = version.ToString(), id = foodDto.Id },
                _linkService.ExpandSingleFoodItem(foodDto, foodDto.Id, version));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateFood))]
        public async Task<ActionResult<FoodDto>> PartiallyUpdateFood(ApiVersion version, int id, [FromBody] JsonPatchDocument<FoodUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var existingDto = await _foodService.GetFoodByIdAsync(id);

            if (existingDto == null)
            {
                return NotFound();
            }

            FoodUpdateDto foodUpdateDto = _mapper.Map<FoodUpdateDto>(existingDto);
            patchDoc.ApplyTo(foodUpdateDto);

            TryValidateModel(foodUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedDto = await _foodService.UpdateFoodAsync(id, foodUpdateDto);

            if (updatedDto == null)
            {
                return NotFound();
            }

            return Ok(_linkService.ExpandSingleFoodItem(updatedDto, updatedDto.Id, version));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveFood))]
        public async Task<ActionResult> RemoveFood(int id)
        {
            var result = await _foodService.DeleteFoodAsync(id);

            if (!result)
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

            var updatedDto = await _foodService.UpdateFoodAsync(id, foodUpdateDto);

            if (updatedDto == null)
            {
                return NotFound();
            }

            return Ok(_linkService.ExpandSingleFoodItem(updatedDto, updatedDto.Id, version));
        }

        [HttpGet("GetRandomMeal", Name = nameof(GetRandomMeal))]
        public async Task<ActionResult> GetRandomMeal()
        {
            var foodDtos = await _foodService.GetRandomMealAsync();

            var links = new List<LinkDto>();

            var selfHref = Url.Link(nameof(GetRandomMeal), null)
                ?? $"{Request.Scheme}://{Request.Host}{Request.PathBase}{Request.Path}";
            links.Add(new LinkDto(selfHref, "self", "GET"));

            return Ok(new
            {
                value = foodDtos,
                links = links
            });
        }
    }
}
