using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using MyFood.Infrastructure.Helpers;
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
        public async Task<ActionResult> GetAllIngredients([FromQuery] QueryParameters queryParameters)
        {
            var ingredientDtos = await _ingredientService.GetAllAsync(queryParameters);
            var allItemCount = await _ingredientService.GetTotalCountAsync();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(ingredientDtos);
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleIngredient))]
        public async Task<ActionResult> GetSingleIngredient(int id)
        {
            if (id < 0)
            {
                return BadRequest("ID must be non-negative.");
            }

            var ingredientDto = await _ingredientService.GetByIdAsync(id);

            if (ingredientDto == null)
            {
                return NotFound();
            }

            return Ok(ingredientDto);
        }

        [HttpGet]
        [Route("search", Name = nameof(SearchIngredients))]
        public async Task<ActionResult> SearchIngredients([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Search name is required.");
            }

            var results = await _ingredientService.SearchAsync(name);
            return Ok(results);
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public async Task<ActionResult> AddIngredient([FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            try
            {
                var ingredientDto = await _ingredientService.CreateAsync(ingredientCreateDto);
                return CreatedAtRoute(nameof(GetSingleIngredient),
                    new { id = ingredientDto.Id },
                    ingredientDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateIngredient))]
        public async Task<ActionResult<IngredientDto>> UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
            }

            var updatedDto = await _ingredientService.UpdateAsync(id, ingredientUpdateDto);

            if (updatedDto == null)
            {
                return NotFound();
            }

            return Ok(updatedDto);
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateIngredient))]
        public async Task<ActionResult<IngredientDto>> PartiallyUpdateIngredient(int id, [FromBody] JsonPatchDocument<IngredientUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var existingDto = await _ingredientService.GetByIdAsync(id);
            if (existingDto == null)
            {
                return NotFound();
            }

            IngredientUpdateDto ingredientUpdateDto = _mapper.Map<IngredientUpdateDto>(existingDto);
            patchDoc.ApplyTo(ingredientUpdateDto);

            TryValidateModel(ingredientUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedDto = await _ingredientService.UpdateAsync(id, ingredientUpdateDto);
            return Ok(updatedDto);
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(DeleteIngredient))]
        public async Task<ActionResult> DeleteIngredient(int id)
        {
            var result = await _ingredientService.DeleteAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
