using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Helpers;
using MyFood.Infrastructure.Repositories;
using System.Text.Json;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;

        public IngredientController(IIngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        [HttpGet(Name = nameof(GetAllIngredients))]
        public async Task<ActionResult<List<IngredientEntity>>> GetAllIngredients()
        {
            var ingredients = await _ingredientRepository.GetAllAsync();
            return Ok(ingredients);
        }

        [HttpGet("{id}", Name = nameof(GetIngredientById))]
        public async Task<ActionResult<IngredientEntity>> GetIngredientById(int id)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            return Ok(ingredient);
        }

        [HttpPost(Name = nameof(CreateIngredient))]
        public async Task<ActionResult<IngredientEntity>> CreateIngredient([FromBody] IngredientCreateDto ingredientDto)
        {
            var ingredient = new IngredientEntity
            {
                Name = ingredientDto.Name,
                Quantity = ingredientDto.Quantity
            };

            var createdIngredient = await _ingredientRepository.AddAsync(ingredient);
            return CreatedAtRoute(nameof(GetIngredientById), new { id = createdIngredient.Id }, createdIngredient);
        }

        [HttpPut("{id}", Name = nameof(UpdateIngredient))]
        public async Task<ActionResult<IngredientEntity>> UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientDto)
        {
            var ingredientToUpdate = new IngredientEntity
            {
                Name = ingredientDto.Name,
                Quantity = ingredientDto.Quantity
            };

            var updatedIngredient = await _ingredientRepository.UpdateAsync(id, ingredientToUpdate);
            if (updatedIngredient == null)
            {
                return NotFound();
            }
            return Ok(updatedIngredient);
        }

        [HttpDelete("{id}", Name = nameof(DeleteIngredient))]
        public async Task<ActionResult> DeleteIngredient(int id)
        {
            var result = await _ingredientRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}