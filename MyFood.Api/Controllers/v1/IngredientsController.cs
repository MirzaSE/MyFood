using AutoMapper;
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
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;
        private readonly IFoodService _foodService;
        private readonly IMapper _mapper;
        private readonly ILinkService<IngredientsController> _linkService;

        public IngredientsController(
            IIngredientService ingredientService,
            IFoodService foodService,
            IMapper mapper,
            ILinkService<IngredientsController> linkService
        )
        {
            _ingredientService = ingredientService;
            _foodService = foodService;
            _mapper = mapper;
            _linkService = linkService;
        }

       
        [HttpGet(Name = nameof(GetAllIngredients))]
        public async Task<ActionResult> GetAllIngredients(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            IEnumerable<IngredientDto> ingredients = await _ingredientService.GetAllIngredientsAsync(queryParameters);

            var allItemCount = await _ingredientService.GetTotalIngredientCountAsync();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = ingredients.Select(x => _linkService.ExpandSingleFoodItem(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleIngredient))]
        public async Task<ActionResult> GetSingleIngredient(ApiVersion version, int id)
        {

            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");
            }

            IngredientDto? ingredient = await _ingredientService.GetIngredientByIdAsync(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(_linkService.ExpandSingleFoodItem(ingredient, ingredient.Id, version));
        }

        [HttpGet]
        [Route("search", Name = nameof(SearchByIngredientName))]
        public async Task<ActionResult> SearchByIngredientName(ApiVersion version,[FromQuery] QueryParameters queryParameters, string name)
        {
            var ingredients = await _ingredientService.SearchIngredientsByNameAsync(name);

            var allItemCount = ingredients.Count();
            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = ingredients.Select(x => _linkService.ExpandSingleFoodItem(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links
            });
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public async Task<ActionResult<IngredientDto>> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            if (ingredientCreateDto.FoodEntityId.HasValue && await _foodService.GetFoodByIdAsync(ingredientCreateDto.FoodEntityId.Value) == null)
            {
                return BadRequest($"Food with id {ingredientCreateDto.FoodEntityId.Value} does not exist.");
            }

            IngredientDto newIngredient = await _ingredientService.CreateIngredientAsync(ingredientCreateDto);

            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { version = version.ToString(), id = newIngredient.Id },
                _linkService.ExpandSingleFoodItem(newIngredient, newIngredient.Id, version));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateIngredient))]
        public async Task<ActionResult<IngredientDto>> PartiallyUpdateIngredient(ApiVersion version, int id, [FromBody] JsonPatchDocument<IngredientUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            IngredientDto? existingIngredient = await _ingredientService.GetIngredientByIdAsync(id);

            if (existingIngredient == null)
            {
                return NotFound();
            }

            IngredientUpdateDto ingredientUpdateDto = _mapper.Map<IngredientUpdateDto>(existingIngredient);
            patchDoc.ApplyTo(ingredientUpdateDto);

            TryValidateModel(ingredientUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (ingredientUpdateDto.FoodEntityId.HasValue && await _foodService.GetFoodByIdAsync(ingredientUpdateDto.FoodEntityId.Value) == null)
            {
                return BadRequest($"Food with id {ingredientUpdateDto.FoodEntityId.Value} does not exist.");
            }

            var updatedIngredient = await _ingredientService.UpdateIngredientAsync(id, ingredientUpdateDto);

            if (updatedIngredient == null)
            {
                return NotFound();
            }

            return Ok(_linkService.ExpandSingleFoodItem(updatedIngredient, updatedIngredient.Id, version));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveIngredient))]
        public async Task<ActionResult> RemoveIngredient(int id)
        {
            if (!await _ingredientService.DeleteIngredientAsync(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateIngredient))]
        public async Task<ActionResult<IngredientDto>> UpdateIngredient(ApiVersion version, int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
            }

            if (ingredientUpdateDto.FoodEntityId.HasValue && await _foodService.GetFoodByIdAsync(ingredientUpdateDto.FoodEntityId.Value) == null)
            {
                return BadRequest($"Food with id {ingredientUpdateDto.FoodEntityId.Value} does not exist.");
            }

            var updatedIngredient = await _ingredientService.UpdateIngredientAsync(id, ingredientUpdateDto);

            if (updatedIngredient == null)
            {
                return NotFound();
            }

            return Ok(_linkService.ExpandSingleFoodItem(updatedIngredient, updatedIngredient.Id, version));
        }

    }
}