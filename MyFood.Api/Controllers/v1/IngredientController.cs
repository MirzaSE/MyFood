using AutoMapper;
using Microsoft.AspNetCore.Authorization; // <-- added
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using MyFood.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFood.Api.Controllers.v1
{
    [Authorize] // <-- Protect all endpoints by default
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

        // GET: api/v1/Ingredient
        [AllowAnonymous] // <-- public read access
        [HttpGet(Name = nameof(GetAllIngredients))]
        public async Task<IActionResult> GetAllIngredients()
        {
            var ingredients = await _ingredientRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<IngredientDto>>(ingredients);
            return Ok(dtos);
        }

        // GET: api/v1/Ingredient/5
        [AllowAnonymous] // <-- public read access
        [HttpGet("{id:int}", Name = nameof(GetIngredientById))]
        public async Task<IActionResult> GetIngredientById(int id)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            if (ingredient == null) return NotFound();

            var dto = _mapper.Map<IngredientDto>(ingredient);
            return Ok(dto);
        }

        // POST: api/v1/Ingredient
        [HttpPost(Name = nameof(CreateIngredient))]
        public async Task<IActionResult> CreateIngredient([FromBody] IngredientCreateDto ingredientDto)
        {
            if (ingredientDto == null) return BadRequest();

            var entity = _mapper.Map<IngredientEntity>(ingredientDto);
            var created = await _ingredientRepository.AddAsync(entity);
            var dto = _mapper.Map<IngredientDto>(created);

            return CreatedAtRoute(nameof(GetIngredientById), new { id = dto.Id, version = "1.0" }, dto);
        }

        // PUT: api/v1/Ingredient/5
        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public async Task<IActionResult> UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientDto)
        {
            if (ingredientDto == null) return BadRequest();

            var entity = _mapper.Map<IngredientEntity>(ingredientDto);
            var updated = await _ingredientRepository.UpdateAsync(id, entity);

            if (updated == null) return NotFound();

            var dto = _mapper.Map<IngredientDto>(updated);
            return Ok(dto);
        }

        // DELETE: api/v1/Ingredient/5
        [HttpDelete("{id:int}", Name = nameof(DeleteIngredient))]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var deleted = await _ingredientRepository.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
