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
        _ingredientSqlRepository = ingredientSqlRepository;
        _mapper = mapper;
        _linkService = linkService;
    }

    [HttpGet("get/{id:int}")]
    public async Task<ActionResult<ServiceResponse<IngredientEntity>>> Get(int id)
    {
        var response = await _ingredientSqlRepository.GetSingle(id);
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpGet("all")]
    public async Task<ActionResult<ServiceResponse<List<IngredientEntity>>>> GetAll()
    {
        var response = await _ingredientSqlRepository.GetAll();
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("add")]
    public async Task<ActionResult<ServiceResponse<IngredientEntity>>> Add(IngredientCreateDto item)
    {
        var ingredientEntity = _mapper.Map<IngredientEntity>(item);
        var response = await _ingredientSqlRepository.Add(ingredientEntity);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult<ServiceResponse<bool>>> Delete(int id)
    {
        var response = await _ingredientSqlRepository.Delete(id);
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPut("update/{id:int}")]
    public async Task<ActionResult<ServiceResponse<IngredientEntity>>> Update(IngredientUpdateDto item, int id)
    {
        var response = await _ingredientSqlRepository.Update(item, id);
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
}