using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/foods/{foodId:int}/ingredients")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public IngredientsController(
            IIngredientRepository ingredientRepository,
            IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        // GET api/v1/foods/{foodId}/ingredients
        [HttpGet(Name = nameof(GetAllIngredients))]
        public ActionResult GetAllIngredients(int foodId)
        {
            if (!_ingredientRepository.FoodExists(foodId))
            {
                return NotFound($"Food item with id {foodId} was not found.");
            }

            IEnumerable<IngredientEntity> ingredients = _ingredientRepository.GetAllByFoodId(foodId);
            IEnumerable<IngredientDto> dtos = ingredients.Select(x => _mapper.Map<IngredientDto>(x));

            return Ok(new
            {
                value = dtos,
                totalCount = _ingredientRepository.Count(foodId)
            });
        }

        // GET api/v1/foods/{foodId}/ingredients/{id}
        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult GetSingleIngredient(int foodId, int id)
        {
            if (!_ingredientRepository.FoodExists(foodId))
            {
                return NotFound($"Food item with id {foodId} was not found.");
            }

            IngredientEntity ingredient = _ingredientRepository.GetSingle(id);

            if (ingredient == null || ingredient.FoodId != foodId)
            {
                return NotFound($"Ingredient with id {id} was not found under food item {foodId}.");
            }

            return Ok(_mapper.Map<IngredientDto>(ingredient));
        }

        // POST api/v1/foods/{foodId}/ingredients
        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult<IngredientDto> AddIngredient(ApiVersion version, int foodId, [FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            if (!_ingredientRepository.FoodExists(foodId))
            {
                return NotFound($"Food item with id {foodId} was not found.");
            }

            IngredientEntity toAdd = _mapper.Map<IngredientEntity>(ingredientCreateDto);

            _ingredientRepository.Add(foodId, toAdd);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            IngredientEntity newIngredient = _ingredientRepository.GetSingle(toAdd.Id);
            IngredientDto ingredientDto = _mapper.Map<IngredientDto>(newIngredient);

            return CreatedAtRoute(
                nameof(GetSingleIngredient),
                new { version = version.ToString(), foodId, id = newIngredient.Id },
                ingredientDto);
        }

        // PUT api/v1/foods/{foodId}/ingredients/{id}
        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult<IngredientDto> UpdateIngredient(int foodId, int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
            }

            if (!_ingredientRepository.FoodExists(foodId))
            {
                return NotFound($"Food item with id {foodId} was not found.");
            }

            IngredientEntity existingIngredient = _ingredientRepository.GetSingle(id);

            if (existingIngredient == null || existingIngredient.FoodId != foodId)
            {
                return NotFound($"Ingredient with id {id} was not found under food item {foodId}.");
            }

            _mapper.Map(ingredientUpdateDto, existingIngredient);

            _ingredientRepository.Update(id, existingIngredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return Ok(_mapper.Map<IngredientDto>(existingIngredient));
        }

        // PATCH api/v1/foods/{foodId}/ingredients/{id}
        [HttpPatch]
        [Route("{id:int}", Name = nameof(PartiallyUpdateIngredient))]
        public ActionResult<IngredientDto> PartiallyUpdateIngredient(int foodId, int id, [FromBody] JsonPatchDocument<IngredientUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!_ingredientRepository.FoodExists(foodId))
            {
                return NotFound($"Food item with id {foodId} was not found.");
            }

            IngredientEntity existingIngredient = _ingredientRepository.GetSingle(id);

            if (existingIngredient == null || existingIngredient.FoodId != foodId)
            {
                return NotFound($"Ingredient with id {id} was not found under food item {foodId}.");
            }

            IngredientUpdateDto ingredientUpdateDto = _mapper.Map<IngredientUpdateDto>(existingIngredient);
            patchDoc.ApplyTo(ingredientUpdateDto);

            TryValidateModel(ingredientUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(ingredientUpdateDto, existingIngredient);
            _ingredientRepository.Update(id, existingIngredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Partially updating an ingredient failed on save.");
            }

            return Ok(_mapper.Map<IngredientDto>(existingIngredient));
        }

        // DELETE api/v1/foods/{foodId}/ingredients/{id}
        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveIngredient))]
        public ActionResult RemoveIngredient(int foodId, int id)
        {
            if (!_ingredientRepository.FoodExists(foodId))
            {
                return NotFound($"Food item with id {foodId} was not found.");
            }

            IngredientEntity ingredient = _ingredientRepository.GetSingle(id);

            if (ingredient == null || ingredient.FoodId != foodId)
            {
                return NotFound($"Ingredient with id {id} was not found under food item {foodId}.");
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