using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Helpers;
using MyFood.Infrastructure.Repositories;
using System.Text.Json;

namespace MyFood.Api.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
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

        // GET: api/v1/Ingredients
        [HttpGet(Name = nameof(GetAllIngredients))]
        public ActionResult GetAllIngredients(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<IngredientEntity> foodItems = _ingredientRepository.GetAll(queryParameters).ToList();

            var allItemCount = _ingredientRepository.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = foodItems.Select(x => _linkService.ExpandSingleFoodItem(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }
        // GET: api/v1/Ingredients/5
        [HttpGet("{id:int}")]
        public ActionResult<IngredientEntity> GetIngredient(int id)
        {
            var ingredient = _ingredientRepository.GetSingle(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(ingredient);
        }

        // POST: api/v1/Ingredients
        [HttpPost]
        public ActionResult AddIngredient([FromBody] IngredientEntity ingredient)
        {
            if (ingredient == null)
            {
                return BadRequest();
            }

            _ingredientRepository.Add(ingredient);

            if (!_ingredientRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return CreatedAtAction(nameof(GetIngredient), new { id = ingredient.Id }, ingredient);
        }

        // PUT: api/v1/Ingredients/5
        [HttpPut("{id:int}")]
        public ActionResult UpdateIngredient(int id, [FromBody] IngredientEntity ingredient)
        {
            if (ingredient == null || id != ingredient.Id)
            {
                return BadRequest();
            }

            var existingIngredient = _ingredientRepository.GetSingle(id);

            if (existingIngredient == null)
            {
                return NotFound();
            }

            _ingredientRepository.Update(id, ingredient);

            if (!_ingredientRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        // DELETE: api/v1/Ingredients/5
        [HttpDelete("{id:int}")]
        public ActionResult DeleteIngredient(int id)
        {
            var ingredient = _ingredientRepository.GetSingle(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            _ingredientRepository.Delete(id);

            if (!_ingredientRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }
    }
}