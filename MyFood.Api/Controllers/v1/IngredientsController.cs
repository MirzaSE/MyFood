using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class IngredientsController : ControllerBase
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IMapper _mapper;

    public IngredientsController(IIngredientRepository ingredientRepository, IMapper mapper)
    {
        _ingredientRepository = ingredientRepository;
        _mapper = mapper;
    }

    [HttpGet(Name = nameof(GetAllIngredients))]
    public ActionResult GetAllIngredients()
    {
        var entities = _ingredientRepository.GetAll();
        var dtos = _mapper.Map<IEnumerable<IngredientDto>>(entities);

        return Ok(dtos);
    }

    [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
    public ActionResult GetSingleIngredient(int id)
    {
        var entity = _ingredientRepository.GetById(id);

        if (entity == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<IngredientDto>(entity));
    }

    [HttpPost(Name = nameof(AddIngredient))]
    public ActionResult<IngredientDto> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto ingredientCreateDto)
    {
        if (ingredientCreateDto == null) return BadRequest();

        var entity = _mapper.Map<IngredientEntity>(ingredientCreateDto);
        _ingredientRepository.Add(entity);

        if (!_ingredientRepository.Save())
        {
            throw new Exception("Creating an ingredient failed on save.");
        }

        var dtoToReturn = _mapper.Map<IngredientDto>(entity);

        return CreatedAtRoute(nameof(GetSingleIngredient),
            new { version = version.ToString(), id = dtoToReturn.Id },
            dtoToReturn);
    }

    [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
    public ActionResult<IngredientDto> UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
    {
        if (ingredientUpdateDto == null) return BadRequest();

        var existingEntity = _ingredientRepository.GetById(id);
        if (existingEntity == null) return NotFound();

        _mapper.Map(ingredientUpdateDto, existingEntity);
        _ingredientRepository.Update(id, existingEntity);

        if (!_ingredientRepository.Save())
        {
            throw new Exception("Updating ingredient failed on save.");
        }

        return Ok(_mapper.Map<IngredientDto>(existingEntity));
    }

    [HttpDelete("{id:int}", Name = nameof(RemoveIngredient))]
    public ActionResult RemoveIngredient(int id)
    {
        var entity = _ingredientRepository.GetById(id);
        if (entity == null) return NotFound();

        _ingredientRepository.Delete(id);

        if (!_ingredientRepository.Save())
        {
            throw new Exception("Deleting ingredient failed on save.");
        }

        return NoContent();
    }
}