using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Application;
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

        public IngredientsController(
            IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        [HttpGet(Name = nameof(GetAllIngredients))]
        public async Task<ActionResult> GetAllIngredients([FromQuery] QueryParameters queryParameters)
        {
            var ingredientDtos = await _ingredientService.GetAllAsync(queryParameters);
            var totalCount = await _ingredientService.GetTotalCountAsync();

            var paginationMetadata = new
            {
                totalCount = totalCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = (int)Math.Ceiling(totalCount / (double)queryParameters.PageCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(ingredientDtos);
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public async Task<ActionResult> GetSingleIngredient(int id)
        {
            var dto = await _ingredientService.GetByIdAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

        [HttpGet("search", Name = nameof(SearchIngredients))]
        public async Task<ActionResult> SearchIngredients([FromQuery] QueryParameters queryParameters, string name)
        {
            var results = await _ingredientService.SearchAsync(name);
            var totalCount = results.Count();

            var paginationMetadata = new
            {
                totalCount = totalCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = (int)Math.Ceiling(totalCount / (double)queryParameters.PageCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(results
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount));
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public async Task<ActionResult<IngredientDto>> AddIngredient([FromBody] IngredientCreateDto createDto)
        {
            if (createDto == null)
            {
                return BadRequest();
            }

            var dto = await _ingredientService.CreateAsync(createDto);

            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { id = dto.Id }, dto);
        }

        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public async Task<ActionResult<IngredientDto>> UpdateIngredient(int id, [FromBody] IngredientUpdateDto updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest();
            }

            var dto = await _ingredientService.UpdateAsync(id, updateDto);
            if (dto == null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

        [HttpDelete("{id:int}", Name = nameof(RemoveIngredient))]
        public async Task<ActionResult> RemoveIngredient(int id)
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