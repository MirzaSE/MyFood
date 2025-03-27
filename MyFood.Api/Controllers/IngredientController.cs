using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Entities;
using MyFood.Application.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;
using MyFood.Domain.Entities;

namespace MyFood.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;

        public IngredientController(IIngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        // 1. Get all ingredients
        [HttpGet]
        public async Task<IActionResult> GetIngredients()
        {
            var ingredients = await _ingredientRepository.GetAllAsync();
            return Ok(ingredients);  // Return list of ingredients with 200 OK status
        }

        // 2. Get ingredient by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIngredientById(int id)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();  // Return 404 if the ingredient is not found
            }
            return Ok(ingredient);  // Return the ingredient with 200 OK status
        }

        // 3. Create a new ingredient
        [HttpPost]
        public async Task<IActionResult> CreateIngredient([FromBody] IngredientEntity ingredient)
        {
            if (ingredient == null)
            {
                return BadRequest("Ingredient data is null.");  // Return 400 if the ingredient is null
            }

            var createdIngredient = await _ingredientRepository.AddAsync(ingredient);
            return CreatedAtAction(nameof(GetIngredientById), new { id = createdIngredient.Id }, createdIngredient);  // Return 201 Created
        }

        // 4. Update an existing ingredient
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIngredient(int id, [FromBody] IngredientEntity ingredient)
        {
            if (ingredient == null || ingredient.Id != id)
            {
                return BadRequest("Ingredient data is invalid.");  // Return 400 if there's a mismatch in ID or null data
            }

            var updatedIngredient = await _ingredientRepository.UpdateAsync(ingredient);
            return Ok(updatedIngredient);  // Return the updated ingredient with 200 OK status
        }

        // 5. Delete an ingredient
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var success = await _ingredientRepository.DeleteAsync(id);
            if (!success)
            {
                return NotFound();  // Return 404 if the ingredient wasn't found
            }
            return NoContent();  // Return 204 No Content if the deletion was successful
        }
    }
}


