using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Application.Services;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Helpers;
using MyFood.Infrastructure.Repositories;
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
        public ActionResult GetAllFoods(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<FoodEntity> foodItems = _foodService.GetAll(queryParameters).ToList();

            var allItemCount = _foodService.Count();

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

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleFood))]
        public ActionResult GetSingleFood(ApiVersion version, int id)
        {

            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");
            }

            FoodEntity foodItem = _foodService.GetSingle(id);

            if (foodItem == null)
            {
                return NotFound();
            }

            FoodDto item = _mapper.Map<FoodDto>(foodItem);

            return Ok(_linkService.ExpandSingleFoodItem(item, item.Id, version));
        }

        [HttpGet]
        [Route("search", Name = nameof(SearchByName))]
        public ActionResult SearchByName(ApiVersion version,[FromQuery] QueryParameters queryParameters, string name)
        {
            var foodItems = _foodService.SearchFoodsByName(name);

            var allItemCount = foodItems.Count();
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
        public ActionResult<FoodDto> AddFood(ApiVersion version, [FromBody] FoodCreateDto foodCreateDto)
        {
            if (foodCreateDto == null)
            {
                return BadRequest();
            }

            FoodEntity toAdd = _mapper.Map<FoodEntity>(foodCreateDto);

            FoodEntity created = _foodService.Add(toAdd);

            FoodDto foodDto = _mapper.Map<FoodDto>(created);

            return CreatedAtRoute(nameof(GetSingleFood),
                new { version = version.ToString(), id = created.Id },
                _linkService.ExpandSingleFoodItem(foodDto, foodDto.Id, version));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateFood))]
        public ActionResult<FoodDto> PartiallyUpdateFood(ApiVersion version, int id, [FromBody] JsonPatchDocument<FoodUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            FoodEntity existingEntity = _foodService.GetSingle(id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            FoodUpdateDto foodUpdateDto = _mapper.Map<FoodUpdateDto>(existingEntity);
            patchDoc.ApplyTo(foodUpdateDto);

            TryValidateModel(foodUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(foodUpdateDto, existingEntity);
            FoodEntity updated = _foodService.Update(id, existingEntity);

            FoodDto foodDto = _mapper.Map<FoodDto>(updated);

            return Ok(_linkService.ExpandSingleFoodItem(foodDto, foodDto.Id, version));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveFood))]
        public ActionResult RemoveFood(int id)
        {
            FoodEntity foodItem = _foodService.GetSingle(id);

            if (foodItem == null)
            {
                return NotFound();
            }

            _foodService.Delete(id);

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateFood))]
        public ActionResult<FoodDto> UpdateFood(ApiVersion version, int id, [FromBody] FoodUpdateDto foodUpdateDto)
        {
            if (foodUpdateDto == null)
            {
                return BadRequest();
            }

            var existingFoodItem = _foodService.GetSingle(id);

            if (existingFoodItem == null)
            {
                return NotFound();
            }

            _mapper.Map(foodUpdateDto, existingFoodItem);

            _foodService.Update(id, existingFoodItem);

            FoodDto foodDto = _mapper.Map<FoodDto>(existingFoodItem);

            return Ok(_linkService.ExpandSingleFoodItem(foodDto, foodDto.Id, version));
        }

        [HttpGet("GetRandomMeal", Name = nameof(GetRandomMeal))]
        public ActionResult GetRandomMeal()
        {
            ICollection<FoodEntity> foodItems = _foodService.GetRandomMeal();

            IEnumerable<FoodDto> dtos = foodItems.Select(x => _mapper.Map<FoodDto>(x));

            var links = new List<LinkDto>();

            // self
            links.Add(new LinkDto(Url.Link(nameof(GetRandomMeal), null), "self", "GET"));

            return Ok(new
            {
                value = dtos,
                links = links
            });
        }
    }
}
