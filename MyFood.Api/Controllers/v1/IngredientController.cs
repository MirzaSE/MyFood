using Microsoft.AspNetCore.Mvc;
using MyFood.Api.Data.Entities;
using MyFood.Api.Data.Repositories;
using MyFood.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFood.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;

        public IngredientsController(IIngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        // GET: api/Ingredients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IngredientEntity>>> GetAllIngredients()
        {
            var ingredients = await _ingredientRepository.GetAllAsync();
            return Ok(ingredients);
        }

        // GET: api/Ingredients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IngredientEntity>> GetIngredientById(int id)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            return Ok(ingredient);
        }

        // POST: api/Ingredients
        [HttpPost]
        public async Task<ActionResult<IngredientEntity>> CreateIngredient([FromBody] IngredientEntity ingredient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _ingredientRepository.AddAsync(ingredient);
            return CreatedAtAction(nameof(GetIngredientById), new { id = ingredient.Id }, ingredient);
        }

        // PUT: api/Ingredients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIngredient(int id, [FromBody] IngredientEntity ingredient)
        {
            if (id != ingredient.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _ingredientRepository.UpdateAsync(ingredient);
            return NoContent();
        }

        // DELETE: api/Ingredients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            await _ingredientRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}