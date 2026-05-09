using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using System.Text.Json;

namespace MyFood.Api.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        [HttpGet(Name = nameof(GetAllIngredients))]
        public async Task<ActionResult> GetAllIngredients([FromQuery] QueryParameters queryParameters)
        {
            var ingredients = await _ingredientService.GetAllAsync(queryParameters);
            var totalCount = await _ingredientService.GetTotalCountAsync();

            var paginationMetadata = new
            {
                totalCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = (int)Math.Ceiling((double)totalCount / queryParameters.PageCount)
            };

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(ingredients);
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public async Task<ActionResult> GetSingleIngredient(int id)
        {
            if (id < 0)
            {
                return BadRequest("ID must be non-negative.");
            }

            var ingredient = await _ingredientService.GetByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(ingredient);
        }

        [HttpGet("search", Name = nameof(SearchIngredients))]
        public async Task<ActionResult> SearchIngredients([FromQuery] QueryParameters queryParameters, string name)
        {
            var results = await _ingredientService.SearchAsync(name);
            var totalCount = results.Count();

            var paginationMetadata = new
            {
                totalCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = (int)Math.Ceiling((double)totalCount / queryParameters.PageCount)
            };

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(results);
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public async Task<ActionResult<IngredientDto>> AddIngredient([FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            try
            {
                var created = await _ingredientService.CreateAsync(ingredientCreateDto);
                return CreatedAtRoute(nameof(GetSingleIngredient), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public async Task<ActionResult<IngredientDto>> UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
            }

            var updated = await _ingredientService.UpdateAsync(id, ingredientUpdateDto);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        [HttpDelete("{id:int}", Name = nameof(RemoveIngredient))]
        public async Task<ActionResult> RemoveIngredient(int id)
        {
            var deleted = await _ingredientService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
