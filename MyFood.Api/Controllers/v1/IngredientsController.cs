using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Entities;
using MyFood.Application.Interfaces;
using System.Collections.Generic;

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

        [HttpPost]
        public IActionResult CreateIngredient([FromBody] IngredientEntity ingredient)
        {
            if (ingredient == null)
            {
                return BadRequest("Ingredient cannot be null.");
            }

            _ingredientRepository.AddIngredient(ingredient);
            return CreatedAtAction(nameof(GetIngredientById), new { id = ingredient.Id }, ingredient);
        }

        [HttpGet]
        public IActionResult GetAllIngredients()
        {
            var ingredients = _ingredientRepository.GetAllIngredients();
            return Ok(ingredients);
        }

        [HttpGet("food/{foodId}")]
        public IActionResult GetIngredientsByFoodId(int foodId)
        {
            var ingredients = _ingredientRepository.GetIngredientsByFoodId(foodId);
            if (ingredients == null || ingredients.Count == 0)
            {
                return NotFound($"No ingredients found for Food with ID {foodId}.");
            }
            return Ok(ingredients);
        }

        [HttpGet("{id}")]
        public IActionResult GetIngredientById(int id)
        {
            var ingredient = _ingredientRepository.GetIngredientsByFoodId(id).FirstOrDefault(); 
            if (ingredient == null)
            {
                return NotFound($"Ingredient with ID {id} not found.");
            }
            return Ok(ingredient);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateIngredient(int id, [FromBody] IngredientEntity ingredient)
        {
            if (ingredient == null || ingredient.Id != id)
            {
                return BadRequest("Ingredient data is incorrect.");
            }

            _ingredientRepository.UpdateIngredient(ingredient);
            return NoContent(); 
        }

        // Delete an ingredient
        [HttpDelete("{id}")]
        public IActionResult DeleteIngredient(int id)
        {
            _ingredientRepository.DeleteIngredient(id);
            return NoContent();
        }
    }
}
