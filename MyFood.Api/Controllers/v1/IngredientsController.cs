using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Infrastructure;
using System.Text.Json;
using MyFood.Infrastructure.Helpers;

namespace MyFood.Api.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;
        private readonly ILinkService<IngredientsController> _linkService;

        public IngredientsController(IIngredientService ingredientService, ILinkService<IngredientsController> linkService)
        {
            _ingredientService = ingredientService;
            _linkService = linkService;
        }

        [HttpGet(Name = nameof(GetAllIngredients))]
        public async Task<ActionResult> GetAllIngredients(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            var dtos = await _ingredientService.GetAllAsync(queryParameters);
            var total = await _ingredientService.GetTotalCountAsync();

            var paginationMetadata = new
            {
                totalCount = total,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(total)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, total, version);
            var toReturn = dtos.Select(x => x);

            return Ok(new { value = toReturn, links = links });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetIngredient))]
        public async Task<ActionResult> GetIngredient(ApiVersion version, int id)
        {
            if (id < 0) throw new ArgumentOutOfRangeException(nameof(id));

            var dto = await _ingredientService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            return Ok(dto);
        }

        [HttpGet]
        [Route("search", Name = nameof(Search))]
        public async Task<ActionResult> Search(ApiVersion version, [FromQuery] QueryParameters queryParameters, string name)
        {
            var dtos = await _ingredientService.SearchAsync(name);
            var total = dtos.Count();

            var paginationMetadata = new
            {
                totalCount = total,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(total)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            return Ok(new { value = dtos, links = _linkService.CreateLinksForCollection(queryParameters, total, version) });
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public async Task<ActionResult<IngredientDto>> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto createDto)
        {
            if (createDto == null) return BadRequest();

            var dto = await _ingredientService.CreateAsync(createDto);
            return CreatedAtRoute(nameof(GetIngredient), new { version = version.ToString(), id = dto.Id }, dto);
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateIngredient))]
        public async Task<ActionResult<IngredientDto>> UpdateIngredient(ApiVersion version, int id, [FromBody] IngredientUpdateDto updateDto)
        {
            if (updateDto == null) return BadRequest();

            var updated = await _ingredientService.UpdateAsync(id, updateDto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateIngredient))]
        public async Task<ActionResult<IngredientDto>> PartiallyUpdateIngredient(ApiVersion version, int id, [FromBody] JsonPatchDocument<IngredientUpdateDto> patchDoc)
        {
            if (patchDoc == null) return BadRequest();

            var existing = await _ingredientService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var updateDto = new IngredientUpdateDto();
            // map existing values
            // simpler approach: map properties manually via JSON
            patchDoc.ApplyTo(updateDto);

            TryValidateModel(updateDto);
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _ingredientService.UpdateAsync(id, updateDto);
            return Ok(updated);
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveIngredient))]
        public async Task<ActionResult> RemoveIngredient(int id)
        {
            var result = await _ingredientService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
