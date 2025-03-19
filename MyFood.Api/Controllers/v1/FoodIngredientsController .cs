using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Repositories;
using System.Collections.Generic;

namespace MyFood.Api.Controllers.v1  // Change "v1" to "v2" for v2 controller
{
    [Route("api/v1/[controller]")]  // Change to "api/v2/[controller]" for v2
    [ApiController]
    public class FoodIngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;

        public FoodIngredientsController(IIngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<IngredientEntity>> GetAllIngredients()
        {
            return Ok(_ingredientRepository.GetAllIngredients());
        }

        [HttpGet("{id}")]
        public ActionResult<IngredientEntity> GetIngredient(int id)
        {
            var ingredient = _ingredientRepository.GetIngredientById(id);
            if (ingredient == null)
                return NotFound();

            return Ok(ingredient);
        }

        [HttpPost]
        public ActionResult AddIngredient([FromBody] IngredientEntity ingredient)
        {
            if (ingredient == null)
                return BadRequest();

            _ingredientRepository.AddIngredient(ingredient);
            return CreatedAtAction(nameof(GetIngredient), new { id = ingredient.Id }, ingredient);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateIngredient(int id, [FromBody] IngredientEntity ingredient)
        {
            if (ingredient == null || id != ingredient.Id)
                return BadRequest();

            _ingredientRepository.UpdateIngredient(ingredient);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteIngredient(int id)
        {
            var ingredient = _ingredientRepository.GetIngredientById(id);
            if (ingredient == null)
                return NotFound();

            _ingredientRepository.DeleteIngredient(id);
            return NoContent();
        }
    }
}
