using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using MyFood.Infrastructure.Helpers;
using System.Text.Json;

namespace MyFood.Api.Controllers.v1
{
    //[Authorize]
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

        [HttpPost(Name= nameof(AddIngredient))]
        public ActionResult AddIngredient(IngridientCreateDto ingridientCreateDto)
        {
            var ingredientEntity = _mapper.Map<IngredientEntity>(ingridientCreateDto);
           
            _ingredientRepository.Add(ingredientEntity);
            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating a Ingredint item failed on save.");
            }
            var ingredientDto = _mapper.Map<IngridientDto>(ingredientEntity);
            return Ok(ingredientDto);
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult GetSingleIngredient(int id)
        {

            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");
            }

            var ingredientEntity = _ingredientRepository.GetSingle(id);

            if (ingredientEntity == null)
            {
                return NotFound();
            }

            var ingredientDto = _mapper.Map<IngridientDto>(ingredientEntity);

            return Ok(ingredientDto);
        }

        [HttpGet(Name = nameof(GetAllIngredients))]
        public ActionResult GetAllIngredients([FromQuery] QueryParameters queryParameters)
        {
            var ingredientItems = _ingredientRepository.GetAll(queryParameters).ToList();

            var allItemCount = _ingredientRepository.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var dtoList = ingredientItems.Select(x => _mapper.Map<IngridientDto>(x));

            return Ok(dtoList);
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateIngredient))]
        public ActionResult<IngridientDto> PartiallyUpdateIngredient(int id, [FromBody] JsonPatchDocument<IngridientUpdate> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var existingEntity = _ingredientRepository.GetSingle(id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            IngridientUpdate ingredientUpdateDto = _mapper.Map<IngridientUpdate>(existingEntity);
            patchDoc.ApplyTo(ingredientUpdateDto);

            TryValidateModel(ingredientUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(ingredientUpdateDto, existingEntity);
            IngredientEntity updated = _ingredientRepository.Update(id, existingEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            IngridientDto ingredientDto = _mapper.Map<IngridientDto>(updated);

            return Ok(ingredientDto);
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult<IngridientDto> UpdateIngredient(int id, [FromBody] IngridientUpdate ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
            }

            var existingEntity = _ingredientRepository.GetSingle(id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(ingredientUpdateDto, existingEntity);

            _ingredientRepository.Update(id, existingEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient item failed on save.");
            }

            var ingredientDto = _mapper.Map<IngridientDto>(existingEntity);

            return Ok(ingredientDto);
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveIngredient))]
        public ActionResult RemoveIngredient(int id)
        {
            var ingredientEntity = _ingredientRepository.GetSingle(id);

            if (ingredientEntity == null)
            {
                return NotFound();
            }

            _ingredientRepository.Delete(id);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Deleting an ingredient item failed on save.");
            }

            return NoContent();
        }

    } 
}
