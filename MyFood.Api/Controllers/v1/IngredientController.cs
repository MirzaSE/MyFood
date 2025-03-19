using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Helpers;
using System.Text.Json;
using MyFood.Application.Repositories;
namespace MyFood.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class IngredientController : ControllerBase
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IMapper _mapper;
    
    public IngredientController(
        IIngredientRepository ingredientRepository,
        IMapper mapper)
    {
        _ingredientRepository = ingredientRepository;
        _mapper = mapper;
    }
    
    [HttpGet]
    public ActionResult GetAllIngredients(ApiVersion version, [FromQuery] QueryParameters queryParameters)
    {
        var ingredients = _ingredientRepository.GetAll(queryParameters).ToList();
        var totalCount = _ingredientRepository.Count();

        var paginationMetadata = new
        {
            totalCount,
            pageSize = queryParameters.PageCount,
            currentPage = queryParameters.Page,
            totalPages = queryParameters.GetTotalPages(totalCount)
        };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return Ok(new { value = ingredients });
    }

    [HttpGet("{id}", Name = nameof(GetIngredientById))]
    public ActionResult GetIngredientById(int id, ApiVersion version)
    {
        var ingredient = _ingredientRepository.GetSingle(id);
        if (ingredient == null)
        {
            return NotFound();
        }
        
        return Ok(ingredient);
    }

    [HttpPost]
    public ActionResult CreateIngredient([FromBody] IngredientCreateDto ingredientDto, ApiVersion version)
    {
        var ingredient = _mapper.Map<IngredientEntity>(ingredientDto);
        _ingredientRepository.Add(ingredient);
        _ingredientRepository.Save();

        return CreatedAtRoute(nameof(GetIngredientById), new { id = ingredient.Id, version = version.ToString() }, ingredient);
    }

    [HttpPut("{id}")]
    public ActionResult UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientDto)
    {
        var existingIngredient = _ingredientRepository.GetSingle(id);
        if (existingIngredient == null)
        {
            return NotFound();
        }

        _mapper.Map(ingredientDto, existingIngredient);
        _ingredientRepository.Update(existingIngredient);
        _ingredientRepository.Save();

        return Ok(existingIngredient);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteIngredient(int id)
    {
        var ingredient = _ingredientRepository.GetSingle(id);
        if (ingredient == null)
        {
            return NotFound();
        }

        _ingredientRepository.Delete(id);
        _ingredientRepository.Save();

        return Ok(ingredient);
    }
}