using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using System.Text.Json;

namespace MyFood.Api.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;
        private readonly IMapper _mapper;

        public IngredientsController(
            IIngredientService ingredientService,
            IMapper mapper)
        {
            _ingredientService = ingredientService;
            _mapper = mapper;
        }

        [HttpGet(Name = nameof(GetAllIngredients))]
        public async Task<ActionResult> GetAllIngredients([FromQuery] QueryParameters? queryParameters = null)
        {
            queryParameters ??= new QueryParameters();
            var ingredients = await _ingredientService.GetAllAsync(queryParameters);
            return Ok(ingredients);
        }

        [HttpGet("{id:int}", Name = nameof(GetIngredientById))]
        public async Task<ActionResult> GetIngredientById(int id)
        {
            if (id <= 0)
                return BadRequest("ID must be greater than 0");

            var ingredient = await _ingredientService.GetByIdAsync(id);
            if (ingredient == null)
                return NotFound();

            return Ok(ingredient);
        }

        [HttpPost(Name = nameof(CreateIngredient))]
        public async Task<ActionResult> CreateIngredient(IngredientCreateDto ingredientCreateDto)
        {
            try
            {
                var ingredient = await _ingredientService.CreateAsync(ingredientCreateDto);
                return CreatedAtAction(nameof(GetIngredientById), new { id = ingredient.Id }, ingredient);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public async Task<ActionResult> UpdateIngredient(int id, IngredientUpdateDto ingredientUpdateDto)
        {
            try
            {
                var ingredient = await _ingredientService.UpdateAsync(id, ingredientUpdateDto);
                return Ok(ingredient);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}", Name = nameof(DeleteIngredient))]
        public async Task<ActionResult> DeleteIngredient(int id)
        {
            var result = await _ingredientService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("search", Name = nameof(SearchIngredients))]
        public async Task<ActionResult> SearchIngredients([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest("Search term cannot be empty");

            var results = await _ingredientService.SearchAsync(term);
            return Ok(results);
        }
    }
}
