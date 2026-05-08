using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [Authorize]
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
        public async Task<ActionResult> GetAllIngredients()
        {
            var ingredients = await _ingredientService.GetAllIngredientsAsync();
            return Ok(ingredients);
        }

        [HttpGet("search", Name = nameof(SearchIngredientsByName))]
        public async Task<ActionResult> SearchIngredientsByName(string name)
        {
            var ingredients = await _ingredientService.SearchIngredientsByNameAsync(name);
            return Ok(ingredients);
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public async Task<ActionResult> GetSingleIngredient(int id)
        {
            var ingredientDto = await _ingredientService.GetIngredientByIdAsync(id);
            if (ingredientDto == null)
            {
                return NotFound();
            }

            return Ok(ingredientDto);
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public async Task<ActionResult<IngredientDto>> AddIngredient([FromBody] CreateIngredientDto createIngredientDto)
        {
            if (createIngredientDto == null)
            {
                return BadRequest();
            }

            var ingredientDto = await _ingredientService.CreateIngredientAsync(createIngredientDto);

            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { id = ingredientDto.Id },
                ingredientDto);
        }

        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public async Task<ActionResult<IngredientDto>> UpdateIngredient(int id, [FromBody] UpdateIngredientDto updateIngredientDto)
        {
            if (updateIngredientDto == null)
            {
                return BadRequest();
            }

            var ingredientDto = await _ingredientService.UpdateIngredientAsync(id, updateIngredientDto);
            if (ingredientDto == null)
            {
                return NotFound();
            }

            return Ok(ingredientDto);
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateIngredient))]
        public async Task<ActionResult<IngredientDto>> PartiallyUpdateIngredient(int id, [FromBody] JsonPatchDocument<UpdateIngredientDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var existingIngredientDto = await _ingredientService.GetIngredientByIdAsync(id);

            if (existingIngredientDto == null)
            {
                return NotFound();
            }

            UpdateIngredientDto updateIngredientDto = _mapper.Map<UpdateIngredientDto>(existingIngredientDto);
            patchDoc.ApplyTo(updateIngredientDto);

            TryValidateModel(updateIngredientDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ingredientDto = await _ingredientService.UpdateIngredientAsync(id, updateIngredientDto);

            return Ok(ingredientDto);
        }

        [HttpDelete("{id:int}", Name = nameof(RemoveIngredient))]
        public async Task<ActionResult> RemoveIngredient(int id)
        {
            var result = await _ingredientService.DeleteIngredientAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}