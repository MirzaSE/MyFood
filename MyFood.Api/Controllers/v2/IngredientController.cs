using Microsoft.AspNetCore.Mvc;
using MyFood.Domain.Entities;
using MyFood.Domain.Repositories;

namespace MyFood.Api.Controllers.v2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientRepository _repository;

        public IngredientController(IIngredientRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ingredients = await _repository.GetAllAsync();
            return Ok(ingredients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var ingredient = await _repository.GetByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(ingredient);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IngredientEntity ingredient)
        {
            var result = await _repository.AddAsync(ingredient);
            return CreatedAtAction(nameof(Get), new { id = result.Id, version = "2.0" }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, IngredientEntity ingredient)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            ingredient.Id = id;
            var result = await _repository.UpdateAsync(ingredient);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
