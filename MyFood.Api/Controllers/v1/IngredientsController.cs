using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Helpers;
using MyFood.Infrastructure.Repositories;
using System.Text.Json;

namespace MyFood.Api.Controllers.v1
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [ApiController]
  [ApiVersion("1.0")]
  [Route("api/v{version:apiVersion}/[controller]")]

  public class IngredientsController : ControllerBase
  {
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IMapper _mapper;
    private readonly ILinkService<IngredientsController> _linkService;

    public IngredientsController(
            IIngredientRepository ingredientRepository,
            IMapper mapper,
            ILinkService<IngredientsController> linkService)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
            _linkService = linkService;
        }

    [HttpGet(Name = nameof(GetAllIngredients))]

    public ActionResult GetAllIngredients(ApiVersion version, [FromQuery] QueryParameters queryParameters)
    {
      if (!User.Identity?.IsAuthenticated ?? true)
        return Unauthorized();

      List<IngredientEntity> ingredientItems = _ingredientRepository.GetAll(queryParameters).ToList();

      var allItemCount = _ingredientRepository.Count();

      var paginationMetadata = new
      {
        totalCount = allItemCount,
        pageSize = queryParameters.PageCount,
        currentPage = queryParameters.Page,
        totalPages = queryParameters.GetTotalPages(allItemCount)
      };

      Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata)); 

      var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
      var toReturn = ingredientItems.Select(x => _linkService.ExpandSingleFoodItem(x, x.Id, version));

      return Ok(new
      {
          value = toReturn,
          links = links
      });
    }

    [HttpPost(Name = nameof(AddIngredient))]
    public ActionResult<IngredientDto> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto ingredientCreateDto)
    {
        if (ingredientCreateDto == null)
        {
            return BadRequest();
        }

        var ingredientEntity = _mapper.Map<IngredientEntity>(ingredientCreateDto);

        _ingredientRepository.Add(ingredientEntity);

        if (!_ingredientRepository.Save())
        {
            throw new Exception("Igredient creation failed on save.");
        }

        var ingredientDto = _mapper.Map<IngredientDto>(ingredientEntity);

        return CreatedAtRoute(nameof(GetSingleIngredient), 
            new { version = version.ToString(), id = ingredientDto.Id }, 
            _linkService.ExpandSingleFoodItem(ingredientDto, ingredientDto.Id, version));
    }


    [HttpGet]
    [Route("{id:int}", Name = nameof(GetSingleIngredient))]
    public ActionResult GetSingleIngredient(ApiVersion version, int id)
    {
        var ingredientEntity = _ingredientRepository.GetSingle(id);
        
        if (ingredientEntity == null)
        {
            return NotFound();
        }
        
        var ingredientDto = _mapper.Map<IngredientDto>(ingredientEntity);
        
        return Ok(_linkService.ExpandSingleFoodItem(ingredientDto, ingredientDto.Id, version));
    }

  
    [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveIngredient))]
        public ActionResult RemoveIngredient(int id)
        {
            IngredientEntity ingredientItem = _ingredientRepository.GetSingle(id);

            if (ingredientItem == null)
            {
                return NotFound();
            }

            _ingredientRepository.Delete(id);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Deleting an ingredient failed on save.");
            }

            return NoContent();
        }


    [HttpPut]
    [Route("{id:int}", Name = nameof(UpdateIngredient))]
    public ActionResult<IngredientDto> UpdateIngredient(ApiVersion version, int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
    {
        if (ingredientUpdateDto == null)
        {
            return BadRequest();
        }

        var existingIngredient = _ingredientRepository.GetSingle(id);
        if (existingIngredient == null)
        {
            return NotFound();
        }

        _mapper.Map(ingredientUpdateDto, existingIngredient);

        _ingredientRepository.Update(id, existingIngredient);

        if (!_ingredientRepository.Save())
        {
            throw new Exception("Ingredient update failed on save.");
        }

        var resultDto = _mapper.Map<IngredientDto>(existingIngredient);

        return Ok(_linkService.ExpandSingleFoodItem(resultDto, resultDto.Id, version));
    }

    }
}