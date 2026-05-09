using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
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
        public ActionResult GetAllIngredients()
        {
            var ingredients = _ingredientRepository.GetAll().ToList();
            var result = _mapper.Map<IEnumerable<IngredientDto>>(ingredients);
            return Ok(result);
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult GetSingleIngredient(int id)
        {
            var ingredient = _ingredientRepository.GetSingle(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IngredientDto>(ingredient));
        }

        [HttpPost]
        public ActionResult<IngredientDto> AddIngredient([FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            var foodItem = _foodRepository.GetSingle(ingredientCreateDto.FoodEntityId);

            if (foodItem == null)
            {
                return BadRequest("FoodEntityId does not exist.");
            }

            var ingredientEntity = _mapper.Map<IngredientEntity>(ingredientCreateDto);

            _ingredientRepository.Add(ingredientEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating ingredient failed on save.");
            }

            var ingredientToReturn = _mapper.Map<IngredientDto>(ingredientEntity);

            return CreatedAtRoute(
                nameof(GetSingleIngredient),
                new { id = ingredientToReturn.Id, version = "1.0" },
                ingredientToReturn);
        }

        [HttpPut("{id:int}")]
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

            var foodItem = _foodRepository.GetSingle(ingredientUpdateDto.FoodEntityId);

            if (foodItem == null)
            {
                return BadRequest("FoodEntityId does not exist.");
            }

            _mapper.Map(ingredientUpdateDto, existingIngredient);
            _ingredientRepository.Update(id, existingIngredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating ingredient failed on save.");
            }

            return Ok(_mapper.Map<IngredientDto>(existingIngredient));
        }

        [HttpPatch("{id:int}")]
        public ActionResult PartiallyUpdateIngredient(int id, [FromBody] JsonPatchDocument<IngredientUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var existingIngredient = _ingredientRepository.GetSingle(id);

            if (existingIngredient == null)
            {
                return NotFound();
            }

            var ingredientToPatch = _mapper.Map<IngredientUpdateDto>(existingIngredient);

            patchDoc.ApplyTo(ingredientToPatch, ModelState);

            if (!TryValidateModel(ingredientToPatch))
            {
                return BadRequest(ModelState);
            }

            var foodItem = _foodRepository.GetSingle(ingredientToPatch.FoodEntityId);

            if (foodItem == null)
            {
                return BadRequest("FoodEntityId does not exist.");
            }

            _mapper.Map(ingredientToPatch, existingIngredient);
            _ingredientRepository.Update(id, existingIngredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Patching ingredient failed on save.");
            }

            return Ok(_mapper.Map<IngredientDto>(existingIngredient));
        }

        [HttpDelete("{id:int}")]
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
                throw new Exception("Deleting ingredient failed on save.");
            }

            return NoContent();
        }
    }
}
