using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Repositories;
using Microsoft.AspNetCore.JsonPatch;

namespace MyFood.Api.Controllers.v1
{
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

        [HttpGet]
        public ActionResult<IEnumerable<IngredientDto>> GetAllIngredients()
        {
            var ingredientsFromRepo = _ingredientRepository.GetAll().ToList();
            return Ok(_mapper.Map<IEnumerable<IngredientDto>>(ingredientsFromRepo));
        }

        [HttpGet("{id}", Name = "GetIngredient")]
        public ActionResult<IngredientDto> GetSingleIngredient(int id)
        {
            var ingredientFromRepo = _ingredientRepository.GetSingle(id);

            if (ingredientFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IngredientDto>(ingredientFromRepo));
        }

        [HttpPost]
        public ActionResult<IngredientDto> AddIngredient([FromBody] IngredientCreateDto ingredientCreateDto)
        {
            var ingredientEntity = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            
            _ingredientRepository.Add(ingredientEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            var ingredientToReturn = _mapper.Map<IngredientDto>(ingredientEntity);

            return CreatedAtRoute("GetIngredient", 
                new { version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0", id = ingredientToReturn.Id }, 
                ingredientToReturn);
        }

        [HttpPatch("{id}")]
public ActionResult PartiallyUpdateIngredient(int id, [FromBody] JsonPatchDocument<IngredientUpdateDto> patchDoc)
{
    if (patchDoc == null)
    {
        return BadRequest();
    }

    var ingredientEntity = _ingredientRepository.GetSingle(id);

    if (ingredientEntity == null)
    {
        return NotFound();
    }

    var ingredientToPatch = _mapper.Map<IngredientUpdateDto>(ingredientEntity);

    patchDoc.ApplyTo(ingredientToPatch, ModelState);

    if (!TryValidateModel(ingredientToPatch))
    {
        return ValidationProblem(ModelState);
    }

    _mapper.Map(ingredientToPatch, ingredientEntity);

    _ingredientRepository.Update(ingredientEntity);

    if (!_ingredientRepository.Save())
    {
        throw new Exception($"Patching ingredient {id} failed on save.");
    }

    return NoContent();
}

        [HttpPut("{id}")]
        public ActionResult UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            var ingredientEntity = _ingredientRepository.GetSingle(id);

            if (ingredientEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(ingredientUpdateDto, ingredientEntity);
            
            _ingredientRepository.Update(ingredientEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception($"Updating ingredient {id} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteIngredient(int id)
        {
            var ingredientEntity = _ingredientRepository.GetSingle(id);

            if (ingredientEntity == null)
            {
                return NotFound();
            }

            _ingredientRepository.Delete(ingredientEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception($"Deleting ingredient {id} failed on save.");
            }

            return NoContent();
        }
    }
}