using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using System.Text.Json;

namespace MyFood.Api.Controllers.v1
{
    [AllowAnonymous]
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
        public async Task<ActionResult> GetAllIngredients(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            var ingredientDtos = await _ingredientService.GetAllAsync(queryParameters);
            var allItemCount = ingredientDtos.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(new
            {
                value = ingredientDtos,
                links = new List<object>()
            });
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public async Task<ActionResult> GetSingleIngredient(ApiVersion version, int id)
        {
            if (id < 0)
            {
                return BadRequest("ID must be non-negative.");
            }

            var ingredientDto = await _ingredientService.GetByIdAsync(id);
            if (ingredientDto == null)
            {
                return NotFound();
            }

            return Ok(ingredientDto);
        }

        [HttpGet("search", Name = nameof(SearchByName))]
        public async Task<ActionResult> SearchByName(ApiVersion version, [FromQuery] string name)
        {
            var ingredientDtos = await _ingredientService.SearchAsync(name);
            return Ok(new
            {
                value = ingredientDtos,
                links = new List<object>()
            });
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public async Task<ActionResult<IngredientDto>> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ingredientDto = await _ingredientService.CreateAsync(ingredientCreateDto);
            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { version = version.ToString(), id = ingredientDto.Id },
                ingredientDto);
        }

        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public async Task<ActionResult<IngredientDto>> UpdateIngredient(ApiVersion version, int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
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
