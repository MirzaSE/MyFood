using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using MyFood.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFood.Api.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public IngredientController(IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet(Name = nameof(GetAllIngredients))]
        public async Task<IActionResult> GetAllIngredients()
        {
            var ingredients = await _ingredientRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<IngredientDto>>(ingredients);
            return Ok(dtos);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = nameof(GetIngredientById))]
        public async Task<IActionResult> GetIngredientById(int id)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            if (ingredient == null) return NotFound();
            var dto = _mapper.Map<IngredientDto>(ingredient);
            return Ok(dto);
        }

        [HttpPost(Name = nameof(CreateIngredient))]
        public async Task<IActionResult> CreateIngredient([FromBody] IngredientCreateDto ingredientDto)
        {
            if (ingredientDto == null) return BadRequest();
            var entity = _mapper.Map<IngredientEntity>(ingredientDto);
            var created = await _ingredientRepository.AddAsync(entity);
            var dto = _mapper.Map<IngredientDto>(created);
            return CreatedAtRoute(nameof(GetIngredientById), new { id = dto.Id, version = "1.0" }, dto);
        }

        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public async Task<IActionResult> UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientDto)
        {
            if (ingredientDto == null) return BadRequest();
            var existing = await _ingredientRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(ingredientDto.Name))
                existing.Name = ingredientDto.Name;
            if (ingredientDto.Quantity.HasValue)
                existing.Quantity = ingredientDto.Quantity.Value;
            if (ingredientDto.FoodId.HasValue)
                existing.FoodId = ingredientDto.FoodId.Value;

            var updated = await _ingredientRepository.UpdateAsync(id, existing);
            var dto = _mapper.Map<IngredientDto>(updated);
            return Ok(dto);
        }

        [HttpDelete("{id:int}", Name = nameof(DeleteIngredient))]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var deleted = await _ingredientRepository.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}