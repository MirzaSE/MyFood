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
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IMapper _mapper;
        private readonly ILinkService<IngredientsController> _linkService;

        public IngredientsController(
            IIngredientRepository ingredientRepository,
            IFoodRepository foodRepository,
            IMapper mapper,
            ILinkService<IngredientsController> linkService
        )
        {
            _ingredientRepository = ingredientRepository;
            _foodRepository = foodRepository;
            _mapper = mapper;
            _linkService = linkService;
        }

       
        [HttpGet(Name = nameof(GetAllIngredients))]
        public ActionResult GetAllIngredients(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<IngredientEntity> ingredients = _ingredientRepository.GetAll(queryParameters).ToList();

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
            var toReturn = ingredients.Select(x => _linkService.ExpandSingleFoodItem(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult GetSingleIngredient(ApiVersion version, int id)
        {

            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");
            }

            IngredientEntity ingredient = _ingredientRepository.GetSingle(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            var item = _mapper.Map<IngredientDto>(ingredient);

            return Ok(_linkService.ExpandSingleFoodItem(item, item.Id, version));
        }

        [HttpGet]
        [Route("search", Name = nameof(SearchByIngredientName))]
        public ActionResult SearchByIngredientName(ApiVersion version,[FromQuery] QueryParameters queryParameters, string name)
        {
            var ingredients = _ingredientRepository.SearchIngredientsByName(name);

            var allItemCount = ingredients.Count();
            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = ingredients.Select(x => _linkService.ExpandSingleFoodItem(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links
            });
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult<IngredientDto> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            if (ingredientCreateDto.FoodEntityId.HasValue && _foodRepository.GetSingle(ingredientCreateDto.FoodEntityId.Value) == null)
            {
                return BadRequest($"Food with id {ingredientCreateDto.FoodEntityId.Value} does not exist.");
            }

            IngredientEntity toAdd = _mapper.Map<IngredientEntity>(ingredientCreateDto);

            _ingredientRepository.Add(toAdd);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            IngredientEntity newIngredient = _ingredientRepository.GetSingle(toAdd.Id);
            IngredientDto ingredientDto = _mapper.Map<IngredientDto>(newIngredient);

            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { version = version.ToString(), id = newIngredient.Id },
                _linkService.ExpandSingleFoodItem(ingredientDto, ingredientDto.Id, version));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateIngredient))]
        public ActionResult<IngredientDto> PartiallyUpdateIngredient(ApiVersion version, int id, [FromBody] JsonPatchDocument<IngredientUpdateDto> patchDoc)
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

            IngredientUpdateDto ingredientUpdateDto = _mapper.Map<IngredientUpdateDto>(existingEntity);
            patchDoc.ApplyTo(ingredientUpdateDto);

            TryValidateModel(ingredientUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (ingredientUpdateDto.FoodEntityId.HasValue && _foodRepository.GetSingle(ingredientUpdateDto.FoodEntityId.Value) == null)
            {
                return BadRequest($"Food with id {ingredientUpdateDto.FoodEntityId.Value} does not exist.");
            }

            _mapper.Map(ingredientUpdateDto, existingEntity);
            IngredientEntity updated = _ingredientRepository.Update(id, existingEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            IngredientDto ingredientDto = _mapper.Map<IngredientDto>(updated);

            return Ok(_linkService.ExpandSingleFoodItem(ingredientDto, ingredientDto.Id, version));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveIngredient))]
        public ActionResult RemoveIngredient(int id)
        {
            IngredientEntity ingredientEntity = _ingredientRepository.GetSingle(id);

            if (ingredientEntity == null)
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

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult<IngredientDto> UpdateIngredient(ApiVersion version, int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
            }

            if (ingredientUpdateDto.FoodEntityId.HasValue && _foodRepository.GetSingle(ingredientUpdateDto.FoodEntityId.Value) == null)
            {
                return BadRequest($"Food with id {ingredientUpdateDto.FoodEntityId.Value} does not exist.");
            }

            var existingIngredient = _ingredientRepository.GetSingle(id);

            if (existingIngredient == null)
            {
                return NotFound();
            }

            _mapper.Map(ingredientUpdateDto, existingIngredient);

            _ingredientRepository.Update(id, existingIngredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            IngredientDto ingredientDto = _mapper.Map<IngredientDto>(existingIngredient);

            return Ok(_linkService.ExpandSingleFoodItem(ingredientDto, ingredientDto.Id, version));
        }

    }
}