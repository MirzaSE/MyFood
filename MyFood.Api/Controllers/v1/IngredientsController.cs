using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Infrastructure.Helpers;
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
            var totalCount = await _ingredientService.GetTotalCountAsync(queryParameters.Query);

            Response.Headers["X-Pagination"] = JsonSerializer.Serialize(new
            {
                totalCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(totalCount)
            });

            return Ok(new { value = ingredients });
        }

        [HttpGet("{id:int}", Name = nameof(GetIngredientById))]
        public async Task<ActionResult> GetIngredientById(int id)
        {
            if (id < 0)
            {
                return BadRequest("ID must be non-negative.");
            }

            var ingredient = await _ingredientService.GetByIdAsync(id);
            if (ingredient is null)
            {
                return NotFound();
            }

            return Ok(ingredient);
        }

        [HttpGet("search", Name = nameof(SearchIngredients))]
        public async Task<ActionResult> SearchIngredients([FromQuery] string name)
        {
            var ingredients = await _ingredientService.SearchAsync(name);
            return Ok(new { value = ingredients });
        }

        [HttpPost(Name = nameof(CreateIngredient))]
        public async Task<ActionResult> CreateIngredient(ApiVersion version, [FromBody] IngredientCreateDto createDto)
        {
            var created = await _ingredientService.CreateAsync(createDto);
            return CreatedAtRoute(nameof(GetIngredientById), new { version = version.ToString(), id = created.Id }, created);
        }

        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public async Task<ActionResult> UpdateIngredient(int id, [FromBody] IngredientUpdateDto updateDto)
        {
            var updated = await _ingredientService.UpdateAsync(id, updateDto);
            if (updated is null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        [HttpDelete("{id:int}", Name = nameof(DeleteIngredient))]
        public async Task<ActionResult> DeleteIngredient(int id)
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
