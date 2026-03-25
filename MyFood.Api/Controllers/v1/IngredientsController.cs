using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Helpers;
using MyFood.Infrastructure.Repositories;
using System.Text.Json;

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
        public ActionResult GetAllIngredients(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<IngredientEntity> ingredientItems = _ingredientRepository.GetAll(queryParameters).ToList();

            var allItemCount = _ingredientRepository.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentSize = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = ingredientItems.Select(x => _linkService.ExpandSingleIngredientItem(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links = links
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

            IngredientEntity ingredientItem = _ingredientRepository.GetSingle(id);

            if (ingredientItem == null)
            {
                return NotFound();
            }

            IngredientDto item = _mapper.Map<IngredientDto>(ingredientItem);

            return Ok(_linkService.ExpandSingleIngredientItem(item, item.Id, version));
        }

        [HttpGet]
        [Route("search", Name = nameof(SearchIngredientsByName))]
        public ActionResult SearchIngredientsByName(ApiVersion version, [FromQuery] QueryParameters queryParameters, string name)
        {
            var ingredientItems = _ingredientRepository.SearchIngredientsByName(name);

            var allItemCount = ingredientItems.Count();
            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = ingredientItems.Select(x => _linkService.ExpandSingleIngredientItem(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult<IngredientDto> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            IngredientEntity toAdd = _mapper.Map<IngredientEntity>(ingredientCreateDto);

            _ingredientRepository.Add(toAdd);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredientitem failed on save.");
            }

            IngredientEntity newIngredientItem = _ingredientRepository.GetSingle(toAdd.Id);
            IngredientDto ingredientDto = _mapper.Map<IngredientDto>(newIngredientItem);

            return CreatedAtRoute(nameof(GetSingleIngredient),
            new { version = version.ToString(), id = newIngredientItem.Id }, _linkService.ExpandSingleIngredientItem(ingredientDto, ingredientDto.Id, version));
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

            _mapper.Map(ingredientUpdateDto, existingEntity);
            IngredientEntity updated = _ingredientRepository.Update(id, existingEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredientitem failed at save.");
            }

            IngredientDto ingredientDto = _mapper.Map<IngredientDto>(updated);

            return Ok(_linkService.ExpandSingleIngredientItem(ingredientDto, ingredientDto.Id, version));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveIngredient))]
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
                throw new Exception("Deleting an ingredient item failed on save.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult<FoodDto> UpdateIngredient(ApiVersion version, int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
            }

            var existingIngredientItem = _ingredientRepository.GetSingle(id);

            if (existingIngredientItem == null)
            {
                return NotFound();
            }

            _mapper.Map(ingredientUpdateDto, existingIngredientItem);

            _ingredientRepository.Update(id, existingIngredientItem);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredientitem failed on save.");
            }

            IngredientDto ingredientDto = _mapper.Map<IngredientDto>(existingIngredientItem);

            return Ok(_linkService.ExpandSingleFoodItem(ingredientDto, ingredientDto.Id, version));
        }
    }
}
