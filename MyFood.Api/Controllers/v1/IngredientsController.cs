using Microsoft.AspNetCore.Mvc;
using MyFood.Domain.Entities;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;

        public IngredientsController(IIngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<IngredientEntity>> GetAllIngredients()
        {
            var ingredients = _ingredientRepository.GetAll();
            return Ok(ingredients);
        }

        [HttpGet("{id}")]
        public ActionResult<IngredientEntity> GetIngredientById(int id)
        {
            var ingredient = _ingredientRepository.GetSingle(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(ingredient);
        }

        [HttpGet("food/{foodId}")]
        public ActionResult<IEnumerable<IngredientEntity>> GetIngredientsByFoodId(int foodId)
        {
            var ingredients = _ingredientRepository.GetByFoodId(foodId);
            return Ok(ingredients);
        }

        [HttpPost]
        public ActionResult AddIngredient([FromBody] IngredientEntity ingredient)
        {
            _ingredientRepository.Add(ingredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            return CreatedAtAction(nameof(GetIngredientById),
                new { id = ingredient.Id, version = "1.0" },
                ingredient);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateIngredient(int id, [FromBody] IngredientEntity ingredient)
        {
            var existingIngredient = _ingredientRepository.GetSingle(id);

            if (existingIngredient == null)
            {
                return NotFound();
            }

            var updatedIngredient = _ingredientRepository.Update(id, ingredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return Ok(updatedIngredient);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteIngredient(int id)
        {
            var existingIngredient = _ingredientRepository.GetSingle(id);

            if (existingIngredient == null)
            {
                return NotFound();
            }

            _ingredientRepository.Delete(id);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Deleting an ingredient failed on save.");
            }

            return NoContent();
        }
    }
}