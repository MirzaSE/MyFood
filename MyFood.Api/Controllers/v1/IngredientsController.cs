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

        [HttpGet]
        public ActionResult<IEnumerable<IngredientDto>> GetAllIngredients()
        {
            var ingredients = _ingredientRepository.GetAll();
            return Ok(_mapper.Map<IEnumerable<IngredientDto>>(ingredients));
        }

        [HttpGet("{id:int}")]
        public ActionResult<IngredientDto> GetSingleIngredient(int id)
        {
            var ingredient = _ingredientRepository.GetSingle(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IngredientDto>(ingredient));
        }

        [HttpGet("food/{foodId:int}")]
        public ActionResult<IEnumerable<IngredientDto>> GetIngredientsByFoodId(int foodId)
        {
            var food = _foodRepository.GetSingle(foodId);

            if (food == null)
            {
                return NotFound($"Food with id {foodId} not found.");
            }

            var ingredients = _ingredientRepository.GetByFoodId(foodId);
            return Ok(_mapper.Map<IEnumerable<IngredientDto>>(ingredients));
        }

        [HttpPost]
        public ActionResult<IngredientDto> AddIngredient([FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            var food = _foodRepository.GetSingle(ingredientCreateDto.FoodId);

            if (food == null)
            {
                return BadRequest($"Food with id {ingredientCreateDto.FoodId} does not exist.");
            }

            var ingredient = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            _ingredientRepository.Add(ingredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating ingredient failed on save.");
            }

            var ingredientDto = _mapper.Map<IngredientDto>(ingredient);

            return CreatedAtAction(
                nameof(GetSingleIngredient),
                new { id = ingredient.Id, version = "1.0" },
                ingredientDto);
        }

        [HttpPut("{id:int}")]
        public ActionResult<IngredientDto> UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
            }

            var existingIngredient = _ingredientRepository.GetSingle(id);

            if (existingIngredient == null)
            {
                return NotFound();
            }

            var food = _foodRepository.GetSingle(ingredientUpdateDto.FoodId);
            if (food == null)
            {
                return BadRequest($"Food with id {ingredientUpdateDto.FoodId} does not exist.");
            }

            _mapper.Map(ingredientUpdateDto, existingIngredient);
            var updatedIngredient = _ingredientRepository.Update(id, existingIngredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating ingredient failed on save.");
            }

            return Ok(_mapper.Map<IngredientDto>(updatedIngredient));
        }

        [HttpDelete("{id:int}")]
        public ActionResult DeleteIngredient(int id)
        {
            var existingIngredient = _ingredientRepository.GetSingle(id);

            if (existingIngredient == null)
            {
                return NotFound();
            }

            _ingredientRepository.Delete(id);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Deleting ingredient failed on save.");
            }

            return NoContent();
        }
    }
}