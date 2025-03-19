using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Repositories;
using MyFood.Infrastructure.Helpers;
using System.Text.Json;
using System.Threading.Tasks;
using MyFood.Application.Repositories;
using MyFood.Application;
using MyFood.Infrastructure;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;
        private readonly ILinkService<IngredientsController> _linkService;

        public IngredientsController(
            IIngredientRepository ingredientRepository,
            IMapper mapper,
            ILinkService<IngredientsController> linkService)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
            _linkService = linkService;
        }

        [HttpGet(Name = nameof(GetAllIngredients))]
        public async Task<ActionResult> GetAllIngredients(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<IngredientEntity> ingredientItems = (await _ingredientRepository.GetAllAsync()).ToList();

            var allItemCount = ingredientItems.Count;

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = ingredientItems.Select(x => _linkService.ExpandSingleFoodItem(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleIngredient))]
        public async Task<ActionResult> GetSingleIngredient(ApiVersion version, int id)
        {
            IngredientEntity ingredientItem = await _ingredientRepository.GetByIdAsync(id);

            if (ingredientItem == null)
            {
                return NotFound();
            }

            // Returning the ingredient entity directly
            return Ok(_linkService.ExpandSingleFoodItem(ingredientItem, ingredientItem.Id, version));
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public async Task<ActionResult> AddIngredient(ApiVersion version, [FromBody] IngredientEntity ingredientEntity)
        {
            if (ingredientEntity == null)
            {
                return BadRequest();
            }

            await _ingredientRepository.AddAsync(ingredientEntity);

            if (!await _ingredientRepository.SaveAsync()) // Assuming SaveAsync method is implemented in your repository
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            IngredientEntity newIngredientItem = await _ingredientRepository.GetByIdAsync(ingredientEntity.Id);

            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { version = version.ToString(), id = newIngredientItem.Id },
                _linkService.ExpandSingleFoodItem(newIngredientItem, newIngredientItem.Id, version));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateIngredient))]
        public async Task<ActionResult> PartiallyUpdateIngredient(ApiVersion version, int id, [FromBody] JsonPatchDocument<IngredientEntity> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            IngredientEntity existingEntity = await _ingredientRepository.GetByIdAsync(id);

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

            await _ingredientRepository.UpdateAsync(existingEntity);

            if (!await _ingredientRepository.SaveAsync())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return Ok(_linkService.ExpandSingleFoodItem(existingEntity, existingEntity.Id, version));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveIngredient))]
        public async Task<ActionResult> RemoveIngredient(int id)
        {
            IngredientEntity ingredientItem = await _ingredientRepository.GetByIdAsync(id);

            if (ingredientItem == null)
            {
                return NotFound();
            }

            await _ingredientRepository.DeleteAsync(id);

            if (!await _ingredientRepository.SaveAsync())
            {
                throw new Exception("Deleting an ingredient failed on save.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateIngredient))]
        public async Task<ActionResult> UpdateIngredient(ApiVersion version, int id, [FromBody] IngredientEntity ingredientEntity)
        {
            if (ingredientEntity == null)
            {
                return BadRequest();
            }

            var existingIngredientItem = await _ingredientRepository.GetByIdAsync(id);

            if (existingIngredientItem == null)
            {
                return NotFound();
            }

            await _ingredientRepository.UpdateAsync(ingredientEntity);

            if (!await _ingredientRepository.SaveAsync())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return Ok(_linkService.ExpandSingleFoodItem(ingredientEntity, ingredientEntity.Id, version));
        }
    }
}
