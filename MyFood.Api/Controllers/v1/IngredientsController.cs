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
    private readonly ILinkService<IngredientsController> _linkService;

    public IngredientsController(
            IIngredientService ingredientService,
            ILinkService<IngredientsController> linkService)
        {
            _ingredientService = ingredientService;
            _linkService = linkService;
        }

    [HttpGet(Name = nameof(GetAllIngredients))]

    public async Task<ActionResult> GetAllIngredients(ApiVersion version, [FromQuery] QueryParameters queryParameters)
    {
      var ingredientDtos = await _ingredientService.GetAllAsync(queryParameters);
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
      var toReturn = ingredientDtos.Select(x => _linkService.ExpandSingleFoodItem(x, x.Id, version));

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

        var ingredientDto = await _ingredientService.CreateAsync(ingredientCreateDto);

        return CreatedAtRoute(nameof(GetSingleIngredient), 
            new { version = version.ToString(), id = ingredientDto.Id }, 
            _linkService.ExpandSingleFoodItem(ingredientDto, ingredientDto.Id, version));
    }


    [HttpGet]
    [Route("{id:int}", Name = nameof(GetSingleIngredient))]
    public async Task<ActionResult> GetSingleIngredient(ApiVersion version, int id)
    {
        var ingredientDto = await _ingredientService.GetByIdAsync(id);

        if (ingredientDto == null)
        {
            return NotFound();
        }

        return Ok(_linkService.ExpandSingleFoodItem(ingredientDto, ingredientDto.Id, version));
    }

  
    [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveIngredient))]
        public async Task<ActionResult> RemoveIngredient(int id)
        {
            var deleted = await _ingredientService.DeleteAsync(id);
            if (!deleted)
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

        var resultDto = await _ingredientService.UpdateAsync(id, ingredientUpdateDto);
        if (resultDto == null)
        {
            return NotFound();
        }

        return Ok(_linkService.ExpandSingleFoodItem(resultDto, resultDto.Id, version));
    }

    }
}