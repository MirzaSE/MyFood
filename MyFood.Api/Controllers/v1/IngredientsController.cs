using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
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
        private readonly IFoodRepository _foodRepository;
        private readonly IMapper _mapper;

        public IngredientsController(
            IIngredientRepository ingredientRepository,
            IFoodRepository foodRepository,
            IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _foodRepository = foodRepository;
            _mapper = mapper;
        }

        [HttpGet(Name = nameof(GetAllIngredients))]
        public ActionResult<IEnumerable<IngredientDto>> GetAllIngredients()
        {
            var ingredients = _ingredientRepository.GetAll();
            return Ok(_mapper.Map<IEnumerable<IngredientDto>>(ingredients));
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult<IngredientDto> GetSingleIngredient(ApiVersion version, int id)
        {
            if (id <= 0)
            {
                return BadRequest("Ingredient id must be greater than zero.");
            }

            var ingredient = _ingredientRepository.GetSingle(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IngredientDto>(ingredient));
        }

        [HttpGet("food/{foodId:int}", Name = nameof(GetIngredientsByFood))]
        public ActionResult<IEnumerable<IngredientDto>> GetIngredientsByFood(int foodId)
        {
            if (foodId <= 0)
            {
                return BadRequest("Food id must be greater than zero.");
            }

            var ingredients = _ingredientRepository.GetByFoodId(foodId);
            return Ok(_mapper.Map<IEnumerable<IngredientDto>>(ingredients));
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult<IngredientDto> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            if (_foodRepository.GetSingle(ingredientCreateDto.FoodId) == null)
            {
                return BadRequest("A valid food item is required for the ingredient.");
            }

            var ingredient = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            _ingredientRepository.Add(ingredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating the ingredient failed on save.");
            }

            var createdIngredient = _ingredientRepository.GetSingle(ingredient.Id);
            var ingredientDto = _mapper.Map<IngredientDto>(createdIngredient);

            return CreatedAtRoute(
                nameof(GetSingleIngredient),
                new { version = version.ToString(), id = ingredientDto.Id },
                ingredientDto);
        }

        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult<IngredientDto> UpdateIngredient(ApiVersion version, int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
            }

            if (id <= 0)
            {
                return BadRequest("Ingredient id must be greater than zero.");
            }

            var existingIngredient = _ingredientRepository.GetSingle(id);
            if (existingIngredient == null)
            {
                return NotFound();
            }

            if (_foodRepository.GetSingle(ingredientUpdateDto.FoodId) == null)
            {
                return BadRequest("A valid food item is required for the ingredient.");
            }

            var ingredient = _mapper.Map<IngredientEntity>(ingredientUpdateDto);
            ingredient.Id = id;
            _ingredientRepository.Update(id, ingredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating the ingredient failed on save.");
            }

            return Ok(_mapper.Map<IngredientDto>(_ingredientRepository.GetSingle(id)));
        }

        [HttpDelete("{id:int}", Name = nameof(RemoveIngredient))]
        public ActionResult RemoveIngredient(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Ingredient id must be greater than zero.");
            }

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
