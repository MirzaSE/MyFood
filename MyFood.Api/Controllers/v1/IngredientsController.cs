using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
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
            var ingredients = _ingredientRepository.GetAll();
            var ingredientDtos = _mapper.Map<IEnumerable<IngredientDto>>(ingredients);
            return Ok(ingredientDtos);
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult GetSingleIngredient(int id)
        {
            IngredientEntity ingredient = _ingredientRepository.GetSingle(id);
            if (ingredient == null)
                return NotFound();

            IngredientDto dto = _mapper.Map<IngredientDto>(ingredient);
            return Ok(dto);
        }

        [HttpGet("food/{foodId:int}", Name = nameof(GetIngredientsByFood))]
        public ActionResult GetIngredientsByFood(int foodId)
        {
            var ingredients = _ingredientRepository.GetByFoodId(foodId);
            var dtos = _mapper.Map<IEnumerable<IngredientDto>>(ingredients);
            return Ok(dtos);
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult<IngredientDto> AddIngredient([FromBody] IngredientCreateDto createDto)
        {
            if (createDto == null)
                return BadRequest();

            IngredientEntity toAdd = _mapper.Map<IngredientEntity>(createDto);
            _ingredientRepository.Add(toAdd);

            if (!_ingredientRepository.Save())
                throw new Exception("Creating an ingredient failed on save.");

            IngredientEntity newItem = _ingredientRepository.GetSingle(toAdd.Id);
            IngredientDto dto = _mapper.Map<IngredientDto>(newItem);

            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { id = newItem.Id }, dto);
        }

        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult<IngredientDto> UpdateIngredient(int id, [FromBody] IngredientUpdateDto updateDto)
        {
            if (updateDto == null)
                return BadRequest();

            IngredientEntity existing = _ingredientRepository.GetSingle(id);
            if (existing == null)
                return NotFound();

            _mapper.Map(updateDto, existing);
            IngredientEntity updated = _ingredientRepository.Update(id, existing);

            if (!_ingredientRepository.Save())
                throw new Exception("Updating an ingredient failed on save.");

            IngredientDto dto = _mapper.Map<IngredientDto>(updated);
            return Ok(dto);
        }

        [HttpDelete("{id:int}", Name = nameof(RemoveIngredient))]
        public ActionResult RemoveIngredient(int id)
        {
            IngredientEntity ingredient = _ingredientRepository.GetSingle(id);
            if (ingredient == null)
                return NotFound();

            _ingredientRepository.Delete(id);

            if (!_ingredientRepository.Save())
                throw new Exception("Deleting an ingredient failed on save.");

            return NoContent();
        }
    }
}