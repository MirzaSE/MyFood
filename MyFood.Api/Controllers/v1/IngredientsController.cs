using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet(Name = nameof(GetAllIngredients))]
        public ActionResult GetAllIngredients()
        {
            List<IngredientEntity> ingredients = _ingredientRepository.GetAllIngredientsAsync().Result;
            return Ok(ingredients);
        }

        

        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult AddIngredient([FromBody] FoodDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            var ingredient = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            _ingredientRepository.Add(ingredient);

            return CreatedAtRoute(nameof(GetIngredientById), new { id = ingredient.Id }, ingredient);
        }

        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult UpdateIngredient(int id, [FromBody] FoodUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
            }

            var existingIngredient = _ingredientRepository.GetIngredientByIdAsync(id).Result;
            if (existingIngredient == null)
            {
                return NotFound();
            }

            _mapper.Map(ingredientUpdateDto, existingIngredient);
            _ingredientRepository.Update(existingIngredient).Wait();

            return Ok(existingIngredient);
        }

        [HttpDelete("{id:int}", Name = nameof(DeleteIngredient))]
        public ActionResult DeleteIngredient(int id)
        {
            var ingredient = _ingredientRepository.GetIngredientByIdAsync(id).Result;
            if (ingredient == null)
            {
                return NotFound();
            }

            _ingredientRepository.Delete(id);
            return NoContent();
        }
    }
}
