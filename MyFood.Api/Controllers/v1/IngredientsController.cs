using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Infrastructure.Helpers;
using System.Text.Json;

namespace MyFood.Api.Controllers.v1
{
    [Authorize]
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

        [HttpGet]
        public async Task<ActionResult> GetAllIngredients([FromQuery] QueryParameters queryParameters)
        {
            var totalCount = await _ingredientService.GetTotalCountAsync(queryParameters.Query);
            var ingredients = string.IsNullOrWhiteSpace(queryParameters.Query)
                ? await _ingredientService.GetAllAsync(queryParameters)
                : await _ingredientService.SearchAsync(queryParameters.Query, queryParameters);

            var paginationMetadata = new
            {
                totalCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(totalCount)
            };

            Response.Headers["X-Pagination"] = JsonSerializer.Serialize(paginationMetadata);

            return Ok(new
            {
                value = ingredients
            });
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public async Task<ActionResult> GetSingleIngredient(int id)
        {
            var ingredient = await _ingredientService.GetByIdAsync(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(ingredient);
        }

        [HttpPost]
        public async Task<ActionResult<IngredientDto>> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var ingredient = await _ingredientService.CreateAsync(ingredientCreateDto);
                return CreatedAtRoute(
                    nameof(GetSingleIngredient),
                    new { id = ingredient.Id, version = version.ToString() },
                    ingredient);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var updated = await _ingredientService.UpdateAsync(id, ingredientUpdateDto);
                return updated == null ? NotFound() : Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> PartiallyUpdateIngredient(int id, [FromBody] JsonPatchDocument<IngredientUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var existingIngredient = await _ingredientService.GetByIdAsync(id);
            if (existingIngredient == null)
            {
                return NotFound();
            }

            var ingredientToPatch = new IngredientUpdateDto
            {
                Name = existingIngredient.Name,
                Unit = existingIngredient.Unit,
                CaloriesPerUnit = existingIngredient.CaloriesPerUnit,
                Protein = existingIngredient.Protein,
                Carbs = existingIngredient.Carbs,
                Fat = existingIngredient.Fat
            };

            patchDoc.ApplyTo(ingredientToPatch, ModelState);

            if (!TryValidateModel(ingredientToPatch))
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updated = await _ingredientService.UpdateAsync(id, ingredientToPatch);
                return updated == null ? NotFound() : Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteIngredient(int id)
        {
            var deleted = await _ingredientService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
