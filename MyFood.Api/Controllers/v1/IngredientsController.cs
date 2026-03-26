using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Api.Controllers.v1
{
    [Authorize]
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
        public ActionResult GetAllIngredients()
        {
            var ingredientEntities = _ingredientRepository.GetAll().ToList();
            var ingredients = ingredientEntities.Select(x => _mapper.Map<IngredientDto>(x));

            return Ok(ingredients);
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult GetSingleIngredient(int id)
        {
            var ingredientEntity = _ingredientRepository.GetSingle(id);

            if (ingredientEntity == null)
            {
                return NotFound();
            }

            var ingredient = _mapper.Map<IngredientDto>(ingredientEntity);
            return Ok(ingredient);
        }

        [HttpGet]
        [Route("food/{foodId:int}", Name = nameof(GetIngredientsByFoodId))]
        public ActionResult GetIngredientsByFoodId(int foodId)
        {
            var food = _foodRepository.GetSingle(foodId);

            if (food == null)
            {
                return NotFound("Food item not found.");
            }

            var ingredientEntities = _ingredientRepository.GetByFoodId(foodId).ToList();
            var ingredients = ingredientEntities.Select(x => _mapper.Map<IngredientDto>(x));

            return Ok(ingredients);
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult AddIngredient([FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            var food = _foodRepository.GetSingle(ingredientCreateDto.FoodId);

            if (food == null)
            {
                return BadRequest("Invalid FoodId. Food item does not exist.");
            }

            var ingredientEntity = _mapper.Map<IngredientEntity>(ingredientCreateDto);

            _ingredientRepository.Add(ingredientEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            var createdIngredient = _ingredientRepository.GetSingle(ingredientEntity.Id);
            var ingredientDto = _mapper.Map<IngredientDto>(createdIngredient);

            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { version = "1.0", id = ingredientDto.Id },
                ingredientDto);
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
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
                return BadRequest("Invalid FoodId. Food item does not exist.");
            }

            _mapper.Map(ingredientUpdateDto, existingIngredient);

            _ingredientRepository.Update(id, existingIngredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            var ingredientDto = _mapper.Map<IngredientDto>(existingIngredient);

            return Ok(ingredientDto);
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveIngredient))]
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
                throw new Exception("Deleting an ingredient failed on save.");
            }

            return NoContent();
        }
    }
}
