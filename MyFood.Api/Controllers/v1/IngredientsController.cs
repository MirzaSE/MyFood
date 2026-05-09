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

        // GET: api/v1/ingredients
        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
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

            return Ok(new { value = ingredients });
        }

        // GET: api/v1/ingredients/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<IngredientDto>> GetById(int id)
        {
            if (id < 0) return BadRequest(new { message = "ID must be non-negative." });

            var dto = await _ingredientService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // GET: api/v1/ingredients/search?name=...
        [HttpGet("search")]
        public async Task<ActionResult> Search([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Search term is required." });

            var results = await _ingredientService.SearchAsync(name);
            return Ok(new { value = results });
        }

        // POST: api/v1/ingredients
        [HttpPost]
        public async Task<ActionResult<IngredientDto>> Create([FromBody] IngredientCreateDto dto)
        {
            if (dto == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _ingredientService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id, version = "1.0" }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PUT: api/v1/ingredients/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<IngredientDto>> Update(int id, [FromBody] IngredientUpdateDto dto)
        {
            if (dto == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _ingredientService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE: api/v1/ingredients/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _ingredientService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
