using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Repositories;

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