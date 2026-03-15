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

        [HttpGet("{foodId:int}", Name = nameof(GetAllIngredientsForFood))]
        public ActionResult GetAllIngredientsForFood(int foodId)
        {
            FoodEntity food = _foodRepository.GetSingle(foodId);
            if (food == null)
            {
                return NotFound();
            }

            IEnumerable<IngredientEntity> ingredients = _ingredientRepository.GetAllForFood(foodId);
            return Ok(_mapper.Map<IEnumerable<IngredientDto>>(ingredients));
        }

        [HttpGet("{foodId:int}/ingredient/{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult GetSingleIngredient(int foodId, int id)
        {
            FoodEntity food = _foodRepository.GetSingle(foodId);
            if (food == null)
            {
                return NotFound();
            }

            IngredientEntity ingredient = _ingredientRepository.GetSingle(id);
            if (ingredient == null || ingredient.FoodId != foodId)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IngredientDto>(ingredient));
        }

        [HttpPost("{foodId:int}", Name = nameof(AddIngredient))]
        public ActionResult<IngredientDto> AddIngredient(int foodId, [FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            FoodEntity food = _foodRepository.GetSingle(foodId);
            if (food == null)
            {
                return NotFound();
            }

            IngredientEntity toAdd = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            toAdd.FoodId = foodId;

            _ingredientRepository.Add(toAdd);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            IngredientDto ingredientDto = _mapper.Map<IngredientDto>(toAdd);

            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { foodId = foodId, id = toAdd.Id },
                ingredientDto);
        }

        [HttpPut("{foodId:int}/ingredient/{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult<IngredientDto> UpdateIngredient(int foodId, int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
            }

            FoodEntity food = _foodRepository.GetSingle(foodId);
            if (food == null)
            {
                return NotFound();
            }

            IngredientEntity existingIngredient = _ingredientRepository.GetSingle(id);
            if (existingIngredient == null || existingIngredient.FoodId != foodId)
            {
                return NotFound();
            }

            _mapper.Map(ingredientUpdateDto, existingIngredient);
            _ingredientRepository.Update(id, existingIngredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return Ok(_mapper.Map<IngredientDto>(existingIngredient));
        }

        [HttpDelete("{foodId:int}/ingredient/{id:int}", Name = nameof(DeleteIngredient))]
        public ActionResult DeleteIngredient(int foodId, int id)
        {
            FoodEntity food = _foodRepository.GetSingle(foodId);
            if (food == null)
            {
                return NotFound();
            }

            IngredientEntity ingredient = _ingredientRepository.GetSingle(id);
            if (ingredient == null || ingredient.FoodId != foodId)
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
