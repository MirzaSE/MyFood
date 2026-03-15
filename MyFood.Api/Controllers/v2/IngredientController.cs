using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Entities;
using MyFood.Application.Interfaces;

namespace MyFood.Api.Controllers.v2
{
    [ApiController]
    [Route("api/v2/[controller]")]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;

        public IngredientController(IIngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        // GET: api/v2/ingredient
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IngredientEntity>>> GetAll()
        {
            var ingredients = await _ingredientRepository.GetAllAsync();
            return Ok(ingredients);
        }

        // GET: api/v2/ingredient/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<IngredientEntity>> GetById(int id)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            if (ingredient == null) return NotFound();
            return Ok(ingredient);
        }

        // POST: api/v2/ingredient
        [HttpPost]
        public async Task<ActionResult<IngredientEntity>> Create([FromBody] IngredientEntity ingredient)
        {
            var createdIngredient = await _ingredientRepository.AddAsync(ingredient);
            return CreatedAtAction(nameof(GetById), new { id = createdIngredient.Id }, createdIngredient);
        }

        // PUT: api/v2/ingredient/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<IngredientEntity>> Update(int id, [FromBody] IngredientEntity ingredient)
        {
            var updatedIngredient = await _ingredientRepository.UpdateAsync(id, ingredient);
            if (updatedIngredient == null) return NotFound();
            return Ok(updatedIngredient);
        }

        // DELETE: api/v2/ingredient/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _ingredientRepository.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}