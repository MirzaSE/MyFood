using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Repositories;
using System.Linq;

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

        public IngredientsController(IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }
        [HttpGet(Name = nameof(GetAllIngredients))]
        public ActionResult GetAllIngredients()
        {
            var entities = _ingredientRepository.GetAllIngredients();
            var dtos = entities.Select(e => _mapper.Map<IngredientDto>(e)).ToList();
            return Ok(dtos);
        }
        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult GetSingleIngredient(int id)
        {
            IngredientEntity? entity = _ingredientRepository.GetIngredientById(id);
            if (entity == null) return NotFound();
            IngredientDto dto = _mapper.Map<IngredientDto>(entity);
            return Ok(dto);
        }
        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult<IngredientDto> AddIngredient([FromBody] IngredientCreateDto createDto)
        {
            if(createDto == null) return BadRequest();

            IngredientEntity toAdd = _mapper.Map<IngredientEntity>(createDto);
            _ingredientRepository.AddIngredient(toAdd);

            if(!_ingredientRepository.Save()) throw new Exception("Creating a ingredient failed on save.");

            IngredientEntity? created = _ingredientRepository.GetIngredientById(toAdd.Id);
            if(created == null) throw new Exception("Ingredient was created but could not be retreived.");

            IngredientDto dto = _mapper.Map<IngredientDto>(created);
            return CreatedAtRoute(nameof(GetSingleIngredient), new { id = created.Id }, dto);
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult<IngredientDto> UpdateIngredient(int id, [FromBody] IngredientUpdateDto updateDto)
        {
            if(updateDto == null) return BadRequest();
            IngredientEntity? existingEntity = _ingredientRepository.GetIngredientById(id);
            if(existingEntity == null) return NotFound();

            _mapper.Map(updateDto, existingEntity);
            _ingredientRepository.UpdateIngredient(existingEntity);

            if(!_ingredientRepository.Save()) throw new Exception("Updating a ingredient failed on save.");

            IngredientDto dto = _mapper.Map<IngredientDto>(existingEntity);
            return Ok(dto);
        }
        [HttpDelete]
        [Route("{id:int}", Name = nameof(DeleteIngredient))]
        public ActionResult DeleteIngredient(int id)
        {
            IngredientEntity? entity = _ingredientRepository.GetIngredientById(id);
            if(entity == null) return NotFound();

            _ingredientRepository.DeleteIngredient(id);

            if(!_ingredientRepository.Save()) throw new Exception("Deleting a ingredient failed on save.");

            return NoContent();
        }
    }
}