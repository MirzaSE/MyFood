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
        private readonly IMapper _mapper;

        public IngredientsController(
            IIngredientRepository ingredientRepository,
            IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        [HttpGet(Name = nameof(GetAllIngredients))]
        public ActionResult GetAllIngredients()
        {
            var allIngredients = _ingredientRepository.GetByFoodId(0)
                .ToList()
                .AsEnumerable();

            var toReturn = _mapper.Map<IEnumerable<IngredientDto>>(allIngredients);

            return Ok(new
            {
                value = toReturn
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult GetSingleIngredient(int id)
        {
            if (id < 0)
            {
                return BadRequest("ID must be non-negative.");
            }

            IngredientEntity ingredient = _ingredientRepository.GetSingle(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            IngredientDto ingredientDto = _mapper.Map<IngredientDto>(ingredient);

            return Ok(ingredientDto);
        }

        [HttpGet]
        [Route("food/{foodId:int}", Name = nameof(GetIngredientsByFoodId))]
        public ActionResult GetIngredientsByFoodId(int foodId)
        {
            if (foodId < 0)
            {
                return BadRequest("Food ID must be non-negative.");
            }

            var ingredients = _ingredientRepository.GetByFoodId(foodId);

            var toReturn = _mapper.Map<IEnumerable<IngredientDto>>(ingredients);

            return Ok(new
            {
                value = toReturn
            });
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult<IngredientDto> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            IngredientEntity toAdd = _mapper.Map<IngredientEntity>(ingredientCreateDto);

            _ingredientRepository.Add(toAdd);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            IngredientEntity newIngredient = _ingredientRepository.GetSingle(toAdd.Id);
            IngredientDto ingredientDto = _mapper.Map<IngredientDto>(newIngredient);

            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { version = version.ToString(), id = newIngredient.Id },
                ingredientDto);
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateIngredient))]
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

            _mapper.Map(ingredientUpdateDto, existingIngredient);

            _ingredientRepository.Update(id, existingIngredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            IngredientDto ingredientDto = _mapper.Map<IngredientDto>(existingIngredient);

            return Ok(ingredientDto);
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveIngredient))]
        public ActionResult RemoveIngredient(int id)
        {
            IngredientEntity ingredient = _ingredientRepository.GetSingle(id);

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
