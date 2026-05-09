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

        // GET /api/v1/ingredients?page=1&pageCount=10&query=tom
        [HttpGet(Name = nameof(GetAllIngredients))]
        public async Task<ActionResult> GetAllIngredients([FromQuery] QueryParameters queryParameters)
        {
            var dtos = await _ingredientService.GetAllAsync(queryParameters);
            var totalCount = await _ingredientService.GetTotalCountAsync();

            var paginationMetadata = new
            {
                totalCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = (int)Math.Ceiling(totalCount / (double)queryParameters.PageCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(new { value = dtos });
        }

        // GET /api/v1/ingredients/5
        [HttpGet("{id:int}", Name = nameof(GetIngredientById))]
        public async Task<ActionResult<IngredientDto>> GetIngredientById(int id)
        {
            if (id < 0)
            {
                return BadRequest(new { message = "ID must be non-negative." });
            }

            var dto = await _ingredientService.GetByIdAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

        // GET /api/v1/ingredients/search?name=tom
        [HttpGet("search", Name = nameof(SearchIngredientsByName))]
        public async Task<ActionResult> SearchIngredientsByName([FromQuery] string name)
        {
            var dtos = await _ingredientService.SearchAsync(name ?? string.Empty);
            return Ok(new { value = dtos });
        }

        // POST /api/v1/ingredients
        [HttpPost(Name = nameof(CreateIngredient))]
        public async Task<ActionResult<IngredientDto>> CreateIngredient(ApiVersion version, [FromBody] IngredientCreateDto createDto)
        {
            if (createDto == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var created = await _ingredientService.CreateAsync(createDto);
                return CreatedAtRoute(
                    nameof(GetIngredientById),
                    new { version = version.ToString(), id = created.Id },
                    created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/v1/ingredients/5
        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public async Task<ActionResult<IngredientDto>> UpdateIngredient(int id, [FromBody] IngredientUpdateDto updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest();
            }

            var updated = await _ingredientService.UpdateAsync(id, updateDto);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        // DELETE /api/v1/ingredients/5
        [HttpDelete("{id:int}", Name = nameof(DeleteIngredient))]
        public async Task<IActionResult> DeleteIngredient(int id)
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
