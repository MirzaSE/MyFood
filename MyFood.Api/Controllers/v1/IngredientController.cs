using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFood.Api.Controllers.v1
{
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

        [HttpGet(Name = nameof(GetAllIngredients))]
        public ActionResult<IEnumerable<IngredientDto>> GetAllIngredients()
        {
            var ingredients = _ingredientRepository.GetAll();
            var dtos = _mapper.Map<IEnumerable<IngredientDto>>(ingredients);
            return Ok(dtos);
        }

        [HttpGet("{id}", Name = nameof(GetIngredientById))]
        public ActionResult<IngredientDto> GetIngredientById(int id)
        {
            var ingredient = _ingredientRepository.GetById(id);
            if (ingredient == null)
                return NotFound();
                
            var dto = _mapper.Map<IngredientDto>(ingredient);
            return Ok(dto);
        }

        [HttpPost(Name = nameof(CreateIngredient))]
        public ActionResult<IngredientDto> CreateIngredient([FromBody] IngredientCreateDto createDto)
        {
            if (createDto == null)
                return BadRequest();
                
            var entity = _mapper.Map<IngredientEntity>(createDto);
            _ingredientRepository.Add(entity);
            
            var resultDto = _mapper.Map<IngredientDto>(entity);
            return CreatedAtRoute(
                nameof(GetIngredientById), 
                new { id = entity.Id, version = HttpContext.GetRequestedApiVersion()?.ToString() }, 
                resultDto);
        }

        [HttpPut("{id}", Name = nameof(UpdateIngredient))]
        public ActionResult UpdateIngredient(int id, [FromBody] IngredientUpdateDto updateDto)
        {
            if (updateDto == null)
                return BadRequest();
                
            var existingIngredient = _ingredientRepository.GetById(id);
            if (existingIngredient == null)
                return NotFound();
                
            _mapper.Map(updateDto, existingIngredient);
            _ingredientRepository.Update(existingIngredient);
            
            return NoContent();
        }

        [HttpPatch("{id}", Name = nameof(PartiallyUpdateIngredient))]
        public ActionResult PartiallyUpdateIngredient(int id, [FromBody] JsonPatchDocument<IngredientUpdateDto> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();
                
            var existingIngredient = _ingredientRepository.GetById(id);
            if (existingIngredient == null)
                return NotFound();
                
            var ingredientToPatch = _mapper.Map<IngredientUpdateDto>(existingIngredient);
            patchDoc.ApplyTo(ingredientToPatch, ModelState);
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            _mapper.Map(ingredientToPatch, existingIngredient);
            _ingredientRepository.Update(existingIngredient);
            
            return NoContent();
        }

        [HttpDelete("{id}", Name = nameof(DeleteIngredient))]
        public ActionResult DeleteIngredient(int id)
        {
            var ingredient = _ingredientRepository.GetById(id);
            if (ingredient == null)
                return NotFound();
                
            _ingredientRepository.Delete(id);
            return NoContent();
        }
    }
}