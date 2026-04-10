using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/ingredients")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public IngredientsController(IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper; 
        }

        [HttpGet]
        public async Task<ActionResult<IngredientDto>> GetIngredients()
        {
            var ingredients = await _ingredientRepository.ListIngredientsAsync();
            return Ok(_mapper.Map<IEnumerable<IngredientDto>>(ingredients));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IngredientDto>> GetIngredient(int id)
        {
            var ingredient = await _ingredientRepository.GetIngredientByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<IngredientDto>(ingredient));
        }

        [HttpPost]
        public async Task<ActionResult> CreateIngredient([FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null) 
            {
                return BadRequest();
            }

            IngredientEntity toAdd = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            await _ingredientRepository.AddIngredientAsync(toAdd);
            return CreatedAtAction(nameof(GetIngredient), new { id = toAdd.Id }, _mapper.Map<IngredientDto>(toAdd));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IngredientDto>> UpdateIngredient(int id, [FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null) 
            {
                return BadRequest();
            }

            var existing = await _ingredientRepository.GetIngredientByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            _mapper.Map(ingredientCreateDto, existing);
            existing.Id = id; 

            await _ingredientRepository.UpdateIngredientAsync(existing);
            return Ok(_mapper.Map<IngredientDto>(existing));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteIngredient(int id)
        {
            var ingredient = await _ingredientRepository.GetIngredientByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            await _ingredientRepository.DeleteIngredientAsync(ingredient);
            return NoContent();

        }
    }
}