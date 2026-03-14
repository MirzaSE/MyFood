using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
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
            var ingredients = _ingredientRepository.GetAllIngredients();
            return Ok(_mapper.Map<IEnumerable<IngredientDto>>(ingredients));
        }

        [HttpGet("{id}")]
        public ActionResult<IngredientDto> GetIngredientById(int id)
        {
            var ingredient = _ingredientRepository.GetIngredientById(id);

            if (ingredient == null)
                return NotFound();

            return Ok(_mapper.Map<IngredientDto>(ingredient));
        }

        [HttpPost]
        public ActionResult<IngredientDto> AddIngredient(CreateIngredientDto createIngredientDto)
        {
            var ingredientEntity = _mapper.Map<IngredientEntity>(createIngredientDto);

            _ingredientRepository.AddIngredient(ingredientEntity);
            _ingredientRepository.Save();

            return Ok(_mapper.Map<IngredientDto>(ingredientEntity));
        }

        [HttpPut("{id}")]
        public ActionResult<IngredientDto> UpdateIngredient(int id, UpdateIngredientDto updateIngredientDto)
        {
            var existingIngredient = _ingredientRepository.GetIngredientById(id);

            if (existingIngredient == null)
                return NotFound();

            _mapper.Map(updateIngredientDto, existingIngredient);

            _ingredientRepository.UpdateIngredient(existingIngredient);
            _ingredientRepository.Save();

            return Ok(_mapper.Map<IngredientDto>(existingIngredient));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteIngredient(int id)
        {
            var existingIngredient = _ingredientRepository.GetIngredientById(id);

            if (existingIngredient == null)
                return NotFound();

            _ingredientRepository.DeleteIngredient(id);
            _ingredientRepository.Save();

            return NoContent();
        }
    }
}