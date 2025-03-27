using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Repositories;
using System.Linq;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public IngredientsController(
            IIngredientRepository ingredientRepository,
            IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        // GET: api/v1/Ingredients
        [HttpGet(Name = nameof(GetAllIngredients))]
        public ActionResult GetAllIngredients()
        {
            var ingredients = _ingredientRepository.GetAll().ToList();
            return Ok(ingredients);
        }

        // GET: api/v1/Ingredients/5
        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult GetSingleIngredient(int id)
        {
            var ingredient = _ingredientRepository.GetSingle(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            return Ok(ingredient);
        }

        // POST: api/v1/Ingredients
        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult AddIngredient([FromBody] IngredientEntity ingredient)
        {
            if (ingredient == null)
            {
                return BadRequest();
            }

            _ingredientRepository.Add(ingredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            return CreatedAtRoute(nameof(GetSingleIngredient), new { id = ingredient.Id }, ingredient);
        }

        // PUT: api/v1/Ingredients/5
        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult UpdateIngredient(int id, [FromBody] IngredientEntity ingredient)
        {
            if (ingredient == null)
            {
                return BadRequest();
            }

            var existingIngredient = _ingredientRepository.GetSingle(id);
            if (existingIngredient == null)
            {
                return NotFound();
            }

            // Map the new values into the existing ingredient, or update properties manually.
            var updatedIngredient = _ingredientRepository.Update(id, ingredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return Ok(updatedIngredient);
        }

        // DELETE: api/v1/Ingredients/5
        [HttpDelete("{id:int}", Name = nameof(DeleteIngredient))]
        public ActionResult DeleteIngredient(int id)
        {
            var ingredient = _ingredientRepository.GetSingle(id);
            if (ingredient == null)
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
