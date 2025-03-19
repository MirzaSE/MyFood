using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Repositories;
using System.Text.Json;
using System.Linq;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public IngredientsController(
            IIngredientRepository ingredientRepository,
            IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        [HttpGet(Name = nameof(GetAllIngredients))]
        public ActionResult GetAllIngredients([FromQuery] QueryParameters queryParameters)
        {
            List<IngredientEntity> ingredientItems = _ingredientRepository.GetAll(queryParameters).ToList();

            var allItemCount = _ingredientRepository.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(new
            {
                value = ingredientItems,
                links = new List<LinkDto>() // You can add links if needed
            });
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult GetSingleIngredient(int id)
        {
            IngredientEntity ingredientItem = _ingredientRepository.GetSingle(id);

            if (ingredientItem == null)
            {
                return NotFound();
            }

            return Ok(ingredientItem);
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult AddIngredient([FromBody] IngredientEntity ingredientEntity)
        {
            if (ingredientEntity == null)
            {
                return BadRequest();
            }

            _ingredientRepository.Add(ingredientEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { id = ingredientEntity.Id },
                ingredientEntity);
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateIngredient))]
        public ActionResult PartiallyUpdateIngredient(int id, [FromBody] JsonPatchDocument<IngredientEntity> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            IngredientEntity existingEntity = _ingredientRepository.GetSingle(id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            patchDoc.ApplyTo(existingEntity);

            TryValidateModel(existingEntity);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _ingredientRepository.Update(id, existingEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return Ok(existingEntity);
        }

        [HttpDelete("{id:int}", Name = nameof(RemoveIngredient))]
        public ActionResult RemoveIngredient(int id)
        {
            IngredientEntity ingredientItem = _ingredientRepository.GetSingle(id);

            if (ingredientItem == null)
            {
                return NotFound();
            }

            _ingredientRepository.Delete(id);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Deleting an ingredient failed on save.");
            }

            return NoContent();
        }

        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult UpdateIngredient(int id, [FromBody] IngredientEntity ingredientEntity)
        {
            if (ingredientEntity == null)
            {
                return BadRequest();
            }

            var existingIngredientItem = _ingredientRepository.GetSingle(id);

            if (existingIngredientItem == null)
            {
                return NotFound();
            }

            _mapper.Map(ingredientEntity, existingIngredientItem);

            _ingredientRepository.Update(id, existingIngredientItem);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return Ok(existingIngredientItem);
        }
    }
}