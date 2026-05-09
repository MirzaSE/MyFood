using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/foods/{foodId:int}/ingredients")]
    public class IngredientsController : ControllerBase
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public IngredientsController(
            IFoodRepository foodRepository,
            IIngredientRepository ingredientRepository,
            IMapper mapper)
        {
            _foodRepository = foodRepository;
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        [HttpGet("/api/v{version:apiVersion}/ingredients", Name = nameof(GetAllIngredients))]
        public ActionResult<IEnumerable<IngredientDto>> GetAllIngredients([FromQuery] QueryParameters queryParameters)
        {
            var ingredients = _ingredientRepository.GetAll(queryParameters);
            return Ok(_mapper.Map<IEnumerable<IngredientDto>>(ingredients));
        }

        [HttpGet(Name = nameof(GetIngredientsForFood))]
        public ActionResult<IEnumerable<IngredientDto>> GetIngredientsForFood(int foodId)
        {
            if (_foodRepository.GetSingle(foodId) == null)
            {
                return NotFound();
            }

            var ingredients = _ingredientRepository.GetAllForFood(foodId);

            return Ok(_mapper.Map<IEnumerable<IngredientDto>>(ingredients));
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult<IngredientDto> GetSingleIngredient(int foodId, int id)
        {
            if (_foodRepository.GetSingle(foodId) == null)
            {
                return NotFound();
            }

            var ingredient = _ingredientRepository.GetSingle(foodId, id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IngredientDto>(ingredient));
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult<IngredientDto> AddIngredient(ApiVersion version, int foodId, [FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            if (_foodRepository.GetSingle(foodId) == null)
            {
                return NotFound();
            }

            var ingredient = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            ingredient.FoodEntityId = foodId;

            _ingredientRepository.Add(ingredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            var newIngredient = _ingredientRepository.GetSingle(foodId, ingredient.Id);
            var ingredientDto = _mapper.Map<IngredientDto>(newIngredient);

            return CreatedAtRoute(
                nameof(GetSingleIngredient),
                new { version = version.ToString(), foodId, id = ingredientDto.Id },
                ingredientDto);
        }

        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult<IngredientDto> UpdateIngredient(int foodId, int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
            }

            if (_foodRepository.GetSingle(foodId) == null)
            {
                return NotFound();
            }

            var existingIngredient = _ingredientRepository.GetSingle(foodId, id);

            if (existingIngredient == null)
            {
                return NotFound();
            }

            _mapper.Map(ingredientUpdateDto, existingIngredient);
            existingIngredient.FoodEntityId = foodId;

            _ingredientRepository.Update(existingIngredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return Ok(_mapper.Map<IngredientDto>(existingIngredient));
        }

        [HttpDelete("{id:int}", Name = nameof(RemoveIngredient))]
        public ActionResult RemoveIngredient(int foodId, int id)
        {
            if (_foodRepository.GetSingle(foodId) == null)
            {
                return NotFound();
            }

            var existingIngredient = _ingredientRepository.GetSingle(foodId, id);

            if (existingIngredient == null)
            {
                return NotFound();
            }

            _ingredientRepository.Delete(existingIngredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Deleting an ingredient failed on save.");
            }

            return NoContent();
        }
    }
}
