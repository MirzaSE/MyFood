using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Api.Controllers.v1
{
    //[Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        [HttpGet(Name = nameof(GetAllIngredients))]
        public async Task<ActionResult> GetAllIngredients()
        {
            var ingredients = await _ingredientService.GetAllAsync();
            return Ok(ingredients);
        }

        [HttpGet("search", Name = nameof(SearchIngredients))]
        public async Task<ActionResult> SearchIngredients([FromQuery] string? query)
        {
            var results = await _ingredientService.SearchAsync(query ?? string.Empty);
            return Ok(results);
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleIngredient))]
        public async Task<ActionResult> GetSingleIngredient(int id)
        {
            var ingredient = await _ingredientService.GetByIdAsync(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(ingredient);
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public async Task<ActionResult<IngredientEntity>> AddIngredient([FromBody] IngredientEntity ingredient)
        {
            if (ingredient == null)
            {
                return BadRequest();
            }

            try
            {
                var created = await _ingredientService.CreateAsync(ingredient);
                return CreatedAtRoute(nameof(GetSingleIngredient),
                    new { id = created.Id },
                    created);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return Conflict();
            }
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateIngredient))]
        public async Task<ActionResult<IngredientEntity>> UpdateIngredient(int id, [FromBody] IngredientEntity ingredient)
        {
            if (ingredient == null)
            {
                return BadRequest();
            }

            try
            {
                IngredientEntity? updated = await _ingredientService.UpdateAsync(id, ingredient);

                if (updated == null)
                {
                    return NotFound();
                }

                return Ok(updated);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return Conflict();
            }
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(DeleteIngredient))]
        public async Task<ActionResult> DeleteIngredient(int id)
        {
            var deleted = await _ingredientService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
