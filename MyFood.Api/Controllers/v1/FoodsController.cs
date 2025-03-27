using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Helpers;
using MyFood.Infrastructure.Repositories;
using System.Text.Json;
using Serilog;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class FoodsController : ControllerBase
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IMapper _mapper;
        private readonly ILinkService<FoodsController> _linkService;
        private readonly ILogger<FoodsController> _logger;

        public FoodsController(
            IFoodRepository foodRepository,
            IMapper mapper,
            ILinkService<FoodsController> linkService,
            ILogger<FoodsController> logger)
        {
            _foodRepository = foodRepository;
            _mapper = mapper;
            _linkService = linkService;
            _logger = logger;
        }

        [HttpGet(Name = nameof(GetAllFoods))]
        public ActionResult GetAllFoods(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            _logger.LogInformation("Getting all foods with query parameters: {@QueryParameters}", queryParameters);
            
            List<FoodEntity> foodItems = _foodRepository.GetAll(queryParameters).ToList();
            var allItemCount = _foodRepository.Count();

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

            _logger.LogInformation("Returning {Count} food items", foodItems.Count);
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
            _logger.LogInformation("Getting single food with ID: {FoodId}", id);
            
            if (id < 1)
            {
                _logger.LogWarning("Invalid ID requested: {FoodId} (must be greater than 0)", id);
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than 0");
            }

            FoodEntity foodItem = _foodRepository.GetSingle(id);

            if (foodItem == null)
            {
                _logger.LogWarning("Food item with ID {FoodId} not found", id);
                return NotFound();
            }

            FoodDto item = _mapper.Map<FoodDto>(foodItem);

            _logger.LogInformation("Successfully retrieved food with ID: {FoodId}", id);
            return Ok(_linkService.ExpandSingleFoodItem(item, item.Id, version));
        }

        [HttpGet]
        [Route("search", Name = nameof(SearchByName))]
        public ActionResult SearchByName(ApiVersion version, [FromQuery] QueryParameters queryParameters, string name)
        {
            _logger.LogInformation("Searching for foods with name: {SearchTerm}", name);
            
            var foodItems = _foodRepository.SearchFoodsByName(name);
            var allItemCount = foodItems.Count();

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

            _logger.LogInformation("Found {Count} items matching search term: {SearchTerm}", allItemCount, name);
            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpPost(Name = nameof(AddFood))]
        public ActionResult<FoodDto> AddFood(ApiVersion version, [FromBody] FoodCreateDto foodCreateDto)
        {
            _logger.LogInformation("Attempting to add new food item");
            
            if (foodCreateDto == null)
            {
                _logger.LogWarning("Add food request failed - null DTO provided");
                return BadRequest();
            }

            FoodEntity toAdd = _mapper.Map<FoodEntity>(foodCreateDto);
            _foodRepository.Add(toAdd);

            if (!_foodRepository.Save())
            {
                _logger.LogError("Failed to save new food item");
                throw new Exception("Creating a fooditem failed on save.");
            }

            FoodEntity newFoodItem = _foodRepository.GetSingle(toAdd.Id);
            FoodDto foodDto = _mapper.Map<FoodDto>(newFoodItem);

            _logger.LogInformation("Successfully added new food item with ID: {FoodId}", newFoodItem.Id);
            return CreatedAtRoute(nameof(GetSingleFood),
                new { version = version.ToString(), id = newFoodItem.Id },
                _linkService.ExpandSingleFoodItem(foodDto, foodDto.Id, version));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateFood))]
        public ActionResult<FoodDto> PartiallyUpdateFood(ApiVersion version, int id, [FromBody] JsonPatchDocument<FoodUpdateDto> patchDoc)
        {
            _logger.LogInformation("Attempting to partially update food item with ID: {FoodId}", id);
            
            if (patchDoc == null)
            {
                _logger.LogWarning("Patch request failed - null patch document provided");
                return BadRequest();
            }

            FoodEntity existingEntity = _foodRepository.GetSingle(id);

            if (existingEntity == null)
            {
                _logger.LogWarning("Patch request failed - food item with ID {FoodId} not found", id);
                return NotFound();
            }

            FoodUpdateDto foodUpdateDto = _mapper.Map<FoodUpdateDto>(existingEntity);
            patchDoc.ApplyTo(foodUpdateDto);

            TryValidateModel(foodUpdateDto);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Patch request failed - validation errors: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
                return BadRequest(ModelState);
            }

            _mapper.Map(foodUpdateDto, existingEntity);
            FoodEntity updated = _foodRepository.Update(id, existingEntity);

            if (!_foodRepository.Save())
            {
                _logger.LogError("Failed to save updated food item with ID: {FoodId}", id);
                throw new Exception("Updating a fooditem failed on save.");
            }

            FoodDto foodDto = _mapper.Map<FoodDto>(updated);
            _logger.LogInformation("Successfully updated food item with ID: {FoodId}", id);
            
            return Ok(_linkService.ExpandSingleFoodItem(foodDto, foodDto.Id, version));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveFood))]
        public ActionResult RemoveFood(int id)
        {
            _logger.LogInformation("Attempting to delete food item with ID: {FoodId}", id);
            
            FoodEntity foodItem = _foodRepository.GetSingle(id);

            if (foodItem == null)
            {
                _logger.LogWarning("Delete request failed - food item with ID {FoodId} not found", id);
                return NotFound();
            }

            _foodRepository.Delete(id);

            if (!_foodRepository.Save())
            {
                _logger.LogError("Failed to delete food item with ID: {FoodId}", id);
                throw new Exception("Deleting a fooditem failed on save.");
            }

            _logger.LogInformation("Successfully deleted food item with ID: {FoodId}", id);
            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateFood))]
        public ActionResult<FoodDto> UpdateFood(ApiVersion version, int id, [FromBody] FoodUpdateDto foodUpdateDto)
        {
            _logger.LogInformation("Attempting to update food item with ID: {FoodId}", id);
            
            if (foodUpdateDto == null)
            {
                _logger.LogWarning("Update request failed - null DTO provided");
                return BadRequest();
            }

            var existingFoodItem = _foodRepository.GetSingle(id);

            if (existingFoodItem == null)
            {
                _logger.LogWarning("Update request failed - food item with ID {FoodId} not found", id);
                return NotFound();
            }

            _mapper.Map(foodUpdateDto, existingFoodItem);
            _foodRepository.Update(id, existingFoodItem);

            if (!_foodRepository.Save())
            {
                _logger.LogError("Failed to save updated food item with ID: {FoodId}", id);
                throw new Exception("Updating a fooditem failed on save.");
            }

            FoodDto foodDto = _mapper.Map<FoodDto>(existingFoodItem);
            _logger.LogInformation("Successfully updated food item with ID: {FoodId}", id);
            
            return Ok(_linkService.ExpandSingleFoodItem(foodDto, foodDto.Id, version));
        }

        [HttpGet("GetRandomMeal", Name = nameof(GetRandomMeal))]
        public ActionResult GetRandomMeal()
        {
            _logger.LogInformation("Getting random meal");
            
            ICollection<FoodEntity> foodItems = _foodRepository.GetRandomMeal();
            IEnumerable<FoodDto> dtos = foodItems.Select(x => _mapper.Map<FoodDto>(x));

            var links = new List<LinkDto>();
            links.Add(new LinkDto(Url.Link(nameof(GetRandomMeal), null), "self", "GET"));

            _logger.LogInformation("Returning {Count} random food items", foodItems.Count);
            return Ok(new
            {
                value = dtos,
                links = links
            });
        }
    }
}
