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
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public IngredientsController(IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult AddIngredient(ApiVersion version, [FromBody] IngredientEntity ingredientEntity)
        {
            if (ingredientEntity == null)
            {
                return BadRequest();
            }

            _ingredientRepository.Add(ingredientEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            return CreatedAtRoute(nameof(GetIngredient), new { version = version.ToString(), id = ingredientEntity.Id }, ingredientEntity);
        }

        [HttpGet("{id:int}", Name = nameof(GetIngredient))]
        public ActionResult<IngredientEntity> GetIngredient(int id)
        {
            var ingredient = _ingredientRepository.GetSingle(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(ingredient);
        }

        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult UpdateIngredient(int id, [FromBody] IngredientEntity updatedIngredient)
        {
            if (updatedIngredient == null)
            {
                return BadRequest();
            }

            var existingIngredient = _ingredientRepository.GetSingle(id);
            if (existingIngredient == null)
            {
                return NotFound();
            }

            _mapper.Map(updatedIngredient, existingIngredient);
            _ingredientRepository.Update(existingIngredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return NoContent();
        }

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
