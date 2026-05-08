using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities; // Change to MyFood.Domain.Entities if you did Option 2
using MyFood.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using MyFood.Application.Services;

namespace MyFood.Api.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        // Dependency Injection of your Repository and AutoMapper
        public IngredientsController(IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        // GET: api/v1/ingredients
        [HttpGet]
        public ActionResult<IEnumerable<IngredientDto>> GetAllIngredients()
        {
            var ingredientsFromRepo = _ingredientRepository.GetAll().ToList();
            
            // Map the entities from the DB to DTOs to return to the user
            return Ok(_mapper.Map<IEnumerable<IngredientDto>>(ingredientsFromRepo));
        }

        // GET: api/v1/ingredients/{id}
        [HttpGet("{id}", Name = "GetIngredient")]
        public ActionResult<IngredientDto> GetSingleIngredient(int id)
        {
            var ingredientFromRepo = _ingredientRepository.GetSingle(id);

            if (ingredientFromRepo == null)
            {
                return NotFound(); // Returns 404 if it doesn't exist
            }

            return Ok(_mapper.Map<IngredientDto>(ingredientFromRepo));
        }

        // POST: api/v1/ingredients
        [HttpPost]
        public ActionResult<IngredientDto> AddIngredient([FromBody] IngredientCreateDto ingredientCreateDto)
        {
            // Map the incoming Create DTO to a full Entity
            var ingredientEntity = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            
            _ingredientRepository.Add(ingredientEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            // Map the new entity back to a regular DTO to show the user the new ID
            var ingredientToReturn = _mapper.Map<IngredientDto>(ingredientEntity);

            // Returns a 201 Created status, and points to the GET method above to view it
            return CreatedAtRoute("GetIngredient", 
                new { version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0", id = ingredientToReturn.Id }, 
                ingredientToReturn);
        }

        // PUT: api/v1/ingredients/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            var ingredientEntity = _ingredientRepository.GetSingle(id);

            if (ingredientEntity == null)
            {
                return NotFound();
            }

            // Overwrite the existing entity with the new values from the DTO
            _mapper.Map(ingredientUpdateDto, ingredientEntity);
            
            _ingredientRepository.Update(ingredientEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception($"Updating ingredient {id} failed on save.");
            }

            return NoContent(); // Returns 204 No Content for successful updates
        }

        // DELETE: api/v1/ingredients/{id}
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

            return NoContent(); // Returns 204 No Content for successful deletion
        }
    }
}