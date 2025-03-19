using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Interfaces;
using MyFood.Domain.Entities;

namespace MyFood.Api.Controllers.v2
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _repository;

        public IngredientsController(IIngredientRepository repository)
        {
            _repository = repository;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ingredient = await _repository.GetByIdAsync(id);
            if (ingredient == null) return NotFound();
            return Ok(ingredient);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IngredientEntity ingredient)
        {
            if (ingredient == null) return BadRequest();
            await _repository.AddAsync(ingredient);
            return CreatedAtAction(nameof(GetById), new { id = ingredient.Id }, ingredient);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] IngredientEntity ingredient)
        {
            if (id != ingredient.Id) return BadRequest();
            await _repository.UpdateAsync(ingredient);
            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
