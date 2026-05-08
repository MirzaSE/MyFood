using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;

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
        public async Task<ActionResult<IEnumerable<IngredientDto>>> GetAllIngredients([FromQuery] QueryParameters queryParameters)
        {
            var ingredients = await _ingredientService.GetAllAsync(queryParameters);
            return Ok(ingredients);
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public async Task<ActionResult<IngredientDto>> GetSingleIngredient(int id)
        {
            var ingredientDto = await _ingredientService.GetByIdAsync(id);

            if (ingredientDto == null)
            {
                return NotFound();
            }

            return Ok(ingredientDto);
        }

        [HttpGet("search", Name = nameof(SearchIngredients))]
        public async Task<ActionResult<IEnumerable<IngredientDto>>> SearchIngredients([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query parameter is required.");
            }

            var results = await _ingredientService.SearchAsync(query);
            return Ok(results);
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public async Task<ActionResult<IngredientDto>> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            var ingredientDto = await _ingredientService.CreateAsync(ingredientCreateDto);

            return CreatedAtRoute(
                nameof(GetSingleIngredient),
                new { version = version.ToString(), id = ingredientDto.Id },
                ingredientDto);
        }

        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public async Task<ActionResult<IngredientDto>> UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
            }

            var updatedDto = await _ingredientService.UpdateAsync(id, ingredientUpdateDto);

            if (updatedDto == null)
            {
                return NotFound();
            }

            return Ok(updatedDto);
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
