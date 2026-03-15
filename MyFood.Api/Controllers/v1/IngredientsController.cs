using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
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
        private readonly IMapper _mapper;

        public IngredientsController(IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        [HttpGet(Name = nameof(GetAllIngredients))]
        public ActionResult<IEnumerable<IngredientDto>> GetAllIngredients([FromQuery] QueryParameters queryParameters)
        {
            var ingredients = _ingredientRepository.GetAll(queryParameters).ToList();
            var ingredientDtos = ingredients.Select(x => _mapper.Map<IngredientDto>(x));
            return Ok(ingredientDtos);
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult<IngredientDto> GetSingleIngredient(int id)
        {
            IngredientEntity? ingredientEntity = _ingredientRepository.GetSingle(id);

            if (ingredientEntity == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IngredientDto>(ingredientEntity));
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult<IngredientDto> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            IngredientEntity ingredientEntity = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            _ingredientRepository.Add(ingredientEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            IngredientDto ingredientDto = _mapper.Map<IngredientDto>(ingredientEntity);

            return CreatedAtRoute(
                nameof(GetSingleIngredient),
                new { version = version.ToString(), id = ingredientDto.Id },
                ingredientDto);
        }

        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult<IngredientDto> UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
            }

            IngredientEntity? existingIngredient = _ingredientRepository.GetSingle(id);

            if (existingIngredient == null)
            {
                return NotFound();
            }

            _mapper.Map(ingredientUpdateDto, existingIngredient);
            IngredientEntity updated = _ingredientRepository.Update(id, existingIngredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return Ok(_mapper.Map<IngredientDto>(updated));
        }

        [HttpDelete("{id:int}", Name = nameof(RemoveIngredient))]
        public ActionResult RemoveIngredient(int id)
        {
            IngredientEntity? existingIngredient = _ingredientRepository.GetSingle(id);

            if (existingIngredient == null)
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
