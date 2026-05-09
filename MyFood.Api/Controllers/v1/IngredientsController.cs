using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _service;

        public IngredientsController(IIngredientService service)
        {
            _service = service;
        }

        [HttpGet(Name = nameof(GetAllIngredients))]
        public async Task<ActionResult<IEnumerable<IngredientDto>>> GetAllIngredients([FromQuery] QueryParameters queryParameters)
        {
            var ingredients = await _service.GetAllAsync(queryParameters);
            var total = await _service.GetTotalCountAsync();
            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(new
            {
                currentPage = queryParameters.Page,
                pageSize = queryParameters.PageCount,
                totalCount = total
            }));
            return Ok(ingredients);
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public async Task<ActionResult<IngredientDto>> GetSingleIngredient(int id)
        {
            var ingredient = await _service.GetByIdAsync(id);
            if (ingredient == null)
                return NotFound();
            return Ok(ingredient);
        }

        [HttpGet("search", Name = nameof(SearchIngredients))]
        public async Task<ActionResult<IEnumerable<IngredientDto>>> SearchIngredients([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Search name cannot be empty.");
            var results = await _service.SearchAsync(name);
            return Ok(results);
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public async Task<ActionResult<IngredientDto>> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtRoute(nameof(GetSingleIngredient),
                    new { version = version.ToString(), id = created.Id },
                    created);
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
        public async Task<ActionResult<IngredientDto>> UpdateIngredient(int id, [FromBody] IngredientUpdateDto dto)
        {
            if (dto == null)
                return BadRequest();

            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id:int}", Name = nameof(RemoveIngredient))]
        public async Task<ActionResult> RemoveIngredient(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
