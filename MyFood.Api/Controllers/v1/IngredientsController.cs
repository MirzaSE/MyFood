using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Entities;
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

        [HttpGet(Name = nameof(GetAllIngredients))]
        public ActionResult<IEnumerable<IngredientEntity>> GetAllIngredients()
        {
            var ingredients = _ingredientRepository.GetAll();
            return Ok(ingredients);
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult<IngredientEntity> GetSingleIngredient(ApiVersion version, int id)
        {
            if (id < 0)
            {
                return BadRequest("Ingredient id must be non-negative.");
            }

            var ingredient = _ingredientRepository.GetSingle(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(ingredient);
        }

        [HttpGet("food/{foodId:int}", Name = nameof(GetIngredientsByFood))]
        public ActionResult<IEnumerable<IngredientEntity>> GetIngredientsByFood(int foodId)
        {
            if (foodId < 0)
            {
                return BadRequest("Food id must be non-negative.");
            }

            var ingredients = _ingredientRepository.GetByFoodId(foodId);
            return Ok(ingredients);
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult<IngredientEntity> AddIngredient(ApiVersion version, [FromBody] IngredientEntity ingredient)
        {
            if (ingredient == null)
            {
                return BadRequest();
            }

            _ingredientRepository.Add(ingredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating the ingredient failed on save.");
            }

            var createdIngredient = _ingredientRepository.GetSingle(ingredient.Id);
            return CreatedAtRoute(nameof(GetSingleIngredient), new { version = version.ToString(), id = createdIngredient.Id }, createdIngredient);
        }

        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult<IngredientEntity> UpdateIngredient(ApiVersion version, int id, [FromBody] IngredientEntity ingredient)
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

            ingredient.Id = id;
            _ingredientRepository.Update(id, ingredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating the ingredient failed on save.");
            }

            return Ok(ingredient);
        }

        [HttpDelete("{id:int}", Name = nameof(RemoveIngredient))]
        public ActionResult RemoveIngredient(int id)
        {
            var ingredient = _ingredientRepository.GetSingle(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            _ingredientRepository.Delete(id);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Deleting the ingredient failed on save.");
            }

            return NoContent();
        }
    }
}
