using Microsoft.AspNetCore.Mvc;
using MyFood.Domain.Entities;
using MyFood.Infrastructure.Repositories;
using MyFood.Infrastructure.Helpers;
using System.Linq;
using MyFood.Application;
using AutoMapper;
using MyFood.Application.Dtos;
using System.Collections.Generic;

namespace MyFood.Api.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public IngredientsController(IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] QueryParameters queryParameters)
        {
            var ingredients = _ingredientRepository.GetAll(queryParameters).ToList();

            var ingredientsDto = _mapper.Map<IEnumerable<IngredientDto>>(ingredients);

            return Ok(ingredientsDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetSingle(int id)
        {
            var ingredient = _ingredientRepository.GetSingle(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            var ingredientDto = _mapper.Map<IngredientDto>(ingredient);

            return Ok(ingredientDto);
        }

        [HttpPost]
        public IActionResult Add([FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest("Ingredient object is null");
            }

            var ingredientEntity = _mapper.Map<IngredientEntity>(ingredientCreateDto);

            _ingredientRepository.Add(ingredientEntity);

            if (!_ingredientRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }


            var ingredientToReturn = _mapper.Map<IngredientDto>(ingredientEntity);

            return CreatedAtAction(nameof(GetSingle), new { id = ingredientToReturn.Id, version = "1.0" }, ingredientToReturn);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest("Ingredient object is null");
            }

            var existingIngredient = _ingredientRepository.GetSingle(id);
            if (existingIngredient == null)
            {
                return NotFound();
            }

            _mapper.Map(ingredientUpdateDto, existingIngredient);

            _ingredientRepository.Update(id, existingIngredient);

            if (!_ingredientRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return Ok(_mapper.Map<IngredientDto>(existingIngredient));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingIngredient = _ingredientRepository.GetSingle(id);
            if (existingIngredient == null)
            {
                return NotFound();
            }

            _ingredientRepository.Delete(id);

            if (!_ingredientRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }
    }
}