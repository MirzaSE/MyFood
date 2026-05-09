using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Services.Interfaces;
using MyFood.Application;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryParameters queryParameters)
        {
            var ingredients = await _ingredientService.GetAllAsync(queryParameters);
            return Ok(ingredients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ingredient = await _ingredientService.GetByIdAsync(id);
            if (ingredient == null) return NotFound();
            return Ok(ingredient);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            var results = await _ingredientService.SearchAsync(name);
            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IngredientCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required.");

            var created = await _ingredientService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] IngredientUpdateDto dto)
        {
            var updated = await _ingredientService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _ingredientService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}