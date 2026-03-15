using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Entities;
using MyFood.Application.Interfaces;

namespace MyFood.Api.Controllers.v1
{
    //[Authorize]
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

        [HttpGet(Name = nameof(GetAllIngredients))]
        public ActionResult GetAllIngredients()
        {
            var ingredients = _ingredientRepository.GetAllIngredients();
            return Ok(ingredients);
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult GetSingleIngredient(int id)
        {
            var ingredient = _ingredientRepository.GetIngredientById(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(ingredient);
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult<IngredientEntity> AddIngredient([FromBody] IngredientEntity ingredient)
        {
            if (ingredient == null)
            {
                return BadRequest();
            }

            _ingredientRepository.AddIngredient(ingredient);

            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { id = ingredient.Id },
                ingredient);
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult<IngredientEntity> UpdateIngredient(int id, [FromBody] IngredientEntity ingredient)
        {
            if (ingredient == null)
            {
                return BadRequest();
            }

            var existing = _ingredientRepository.GetIngredientById(id);

            if (existing == null)
            {
                return NotFound();
            }

            ingredient.Id = id;
            _ingredientRepository.UpdateIngredient(ingredient);

            return Ok(ingredient);
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(DeleteIngredient))]
        public ActionResult DeleteIngredient(int id)
        {
            var existing = _ingredientRepository.GetIngredientById(id);

            if (existing == null)
            {
                return NotFound();
            }

            _ingredientRepository.DeleteIngredient(id);

            return NoContent();
        }
    }
}