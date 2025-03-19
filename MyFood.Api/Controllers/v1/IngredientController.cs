using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Helpers;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class IngredientController : ControllerBase
{
    private readonly IIngredientSqlRepository _ingredientSqlRepository;
    private readonly IMapper _mapper;
    private readonly ILinkService<FoodsController> _linkService;

    public IngredientController(
        IIngredientSqlRepository ingredientSqlRepository,
        IMapper mapper,
        ILinkService<FoodsController> linkService
    )
    {
        _ingredientSqlRepository = ingredientSqlRepository ?? throw new ArgumentNullException(nameof(ingredientSqlRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _linkService = linkService ?? throw new ArgumentNullException(nameof(linkService));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ServiceResponse<IngredientEntity>>> GetIngredient(int id)
    {
        var response = await _ingredientSqlRepository.GetSingle(id);
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<IngredientEntity>>>> GetAllIngredients()
    {
        var response = await _ingredientSqlRepository.GetAll();
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<IngredientEntity>>> AddIngredient([FromBody] IngredientCreateDto item)
    {
        if (item == null)
        {
            return BadRequest(new ServiceResponse<IngredientEntity>
            {
                Success = false,
                Message = "Ingredient data is required."
            });
        }

        var ingredientEntity = _mapper.Map<IngredientEntity>(item);
        var response = await _ingredientSqlRepository.Add(ingredientEntity);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(nameof(GetIngredient), new { id = response.Data.Id }, response);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteIngredient(int id)
    {
        var response = await _ingredientSqlRepository.Delete(id);
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ServiceResponse<IngredientEntity>>> UpdateIngredient(int id, [FromBody] IngredientUpdateDto item)
    {
        if (item == null)
        {
            return BadRequest(new ServiceResponse<IngredientEntity>
            {
                Success = false,
                Message = "Ingredient data is required."
            });
        }

        var response = await _ingredientSqlRepository.Update(item, id);
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
}