using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Helpers;
using System.Text.Json;

namespace MyFood.Api.Controllers.v1
{
    //[Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;
        private readonly ILinkService<IngredientController> _linkService;

        public IngredientController(
            IIngredientRepository ingredientRepository,
            IMapper mapper,
            ILinkService<IngredientController> linkService)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
            _linkService = linkService;
        }

       
        [HttpGet(Name = nameof(GetAllIngredients))]
        public async Task<ActionResult> GetAllIngredients(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            var ingredientItems = (await _ingredientRepository.GetAll(queryParameters)).ToList();
            var allItemCount = await _ingredientRepository.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers["X-Pagination"] = JsonSerializer.Serialize(paginationMetadata);

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = ingredientItems.Select(x => _linkService.ExpandSingleFoodItem(x, x.Id, version));

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

            var ingredientItem = await _ingredientRepository.GetSingle(id);
            if (ingredientItem == null)
            {
                return NotFound();
            }

            var item = _mapper.Map<IngredientDto>(ingredientItem);
            return Ok(_linkService.ExpandSingleFoodItem(item, item.Id, version));
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public async Task<ActionResult<IngredientDto>> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            var toAdd = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            await _ingredientRepository.Add(toAdd);

            var ingredientDto = _mapper.Map<IngredientDto>(toAdd);
            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { version = version.ToString(), id = toAdd.Id },
                _linkService.ExpandSingleFoodItem(ingredientDto, ingredientDto.Id, version));
        }


        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveIngredient))]
        public async Task<ActionResult> RemoveIngredient(int id)
        {
            var ingredientItem = await _ingredientRepository.GetSingle(id);
            if (ingredientItem == null)
            {
                return NotFound();
            }

            await _ingredientRepository.Delete(id);
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

            var existingIngredientItem = await _ingredientRepository.GetSingle(id);
            if (existingIngredientItem == null)
            {
                return NotFound();
            }

            _mapper.Map(ingredientUpdateDto, existingIngredientItem);
            await _ingredientRepository.Update(id, existingIngredientItem);

            var ingredientDto = _mapper.Map<IngredientDto>(existingIngredientItem);
            return Ok(_linkService.ExpandSingleFoodItem(ingredientDto, ingredientDto.Id, version));
        }
    }
}
