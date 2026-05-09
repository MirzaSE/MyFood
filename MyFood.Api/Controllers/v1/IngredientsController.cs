using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Helpers;
using System.Text.Json;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;
        private readonly IMapper _mapper;
        private readonly ILinkService<IngredientsController> _linkService;

        public IngredientsController(
            IIngredientService ingredientService,
            IMapper mapper,
            ILinkService<IngredientsController> linkService)
        {
            _ingredientService = ingredientService;
            _mapper = mapper;
            _linkService = linkService;
        }

        [HttpGet(Name = nameof(GetAllIngredients))]
        public async Task<ActionResult> GetAllIngredients(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            var ingredientDtos = await _ingredientService.GetAllIngredientsAsync(queryParameters);
            var allItemCount = await _ingredientService.GetTotalIngredientCountAsync();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = ingredientDtos.Select(x => _linkService.ExpandSingleIngredientItem(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleIngredient))]
        public async Task<ActionResult> GetSingleIngredient(ApiVersion version, int id)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");
            }

            var ingredientDto = await _ingredientService.GetIngredientByIdAsync(id);

            if (ingredientDto == null)
            {
                return NotFound();
            }

            return Ok(_linkService.ExpandSingleIngredientItem(ingredientDto, ingredientDto.Id, version));
        }

        [HttpGet]
        [Route("search", Name = nameof(SearchIngredientsByName))]
        public async Task<ActionResult> SearchIngredientsByName(ApiVersion version, [FromQuery] QueryParameters queryParameters, string name)
        {
            var ingredientDtos = await _ingredientService.SearchIngredientsByNameAsync(name);

            var allItemCount = ingredientDtos.Count();
            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = ingredientDtos.Select(x => _linkService.ExpandSingleIngredientItem(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public async Task<ActionResult<IngredientDto>> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            var ingredientDto = await _ingredientService.CreateIngredientAsync(ingredientCreateDto);

            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { version = version.ToString(), id = ingredientDto.Id },
                _linkService.ExpandSingleIngredientItem(ingredientDto, ingredientDto.Id, version));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateIngredient))]
        public async Task<ActionResult<IngredientDto>> PartiallyUpdateIngredient(ApiVersion version, int id, [FromBody] JsonPatchDocument<IngredientUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var existingDto = await _ingredientService.GetIngredientByIdAsync(id);
            if (existingDto == null)
            {
                return NotFound();
            }

            var ingredientUpdateDto = _mapper.Map<IngredientUpdateDto>(existingDto);
            patchDoc.ApplyTo(ingredientUpdateDto);

            TryValidateModel(ingredientUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedDto = await _ingredientService.UpdateIngredientAsync(id, ingredientUpdateDto);

            return Ok(_linkService.ExpandSingleIngredientItem(updatedDto!, updatedDto!.Id, version));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveIngredient))]
        public async Task<ActionResult> RemoveIngredient(int id)
        {
            var result = await _ingredientService.DeleteIngredientAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateIngredient))]
        public async Task<ActionResult<IngredientDto>> UpdateIngredient(ApiVersion version, int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
            }

            var updatedDto = await _ingredientService.UpdateIngredientAsync(id, ingredientUpdateDto);

            if (updatedDto == null)
            {
                return NotFound();
            }

            return Ok(_linkService.ExpandSingleIngredientItem(updatedDto, updatedDto.Id, version));
        }
    }
}
