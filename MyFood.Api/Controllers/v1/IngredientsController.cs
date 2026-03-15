using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _repository;

        public IngredientsController(IIngredientRepository repository)
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
        public async Task<IActionResult> GetById(int id)
        {
            var ingredient = await _repository.GetByIdAsync(id);

            if (ingredient == null)
                return NotFound();

            return Ok(ingredient);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IngredientEntity ingredient)
        {
            var result = await _repository.AddAsync(ingredient);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, IngredientEntity ingredient)
        {
            ingredient.Id = id;

            var result = await _repository.UpdateAsync(ingredient);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}