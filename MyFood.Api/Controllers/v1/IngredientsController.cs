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
            var ingredients = _ingredientRepository.GetAll(queryParameters).ToList();
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
            var toReturn = ingredients.Select(x => _linkService.ExpandSingleIngredientItem(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult GetSingleIngredient(ApiVersion version, int id)
        {
            var ingredient = _ingredientRepository.GetSingle(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            var ingredientDto = _mapper.Map<IngredientDto>(ingredient);

            return Ok(_linkService.ExpandSingleIngredientItem(ingredientDto, ingredientDto.Id, version));
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult<IngredientDto> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                return BadRequest();
            }

            var toAdd = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            _ingredientRepository.Add(toAdd);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            var newIngredient = _ingredientRepository.GetSingle(toAdd.Id);
            var ingredientDto = _mapper.Map<IngredientDto>(newIngredient);

            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { version = version.ToString(), id = newIngredient.Id },
                _linkService.ExpandSingleIngredientItem(ingredientDto, ingredientDto.Id, version));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateIngredient))]
        public ActionResult<IngredientDto> PartiallyUpdateIngredient(ApiVersion version, int id, [FromBody] JsonPatchDocument<IngredientUpdateDto> patchDoc)
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

            var ingredientUpdateDto = _mapper.Map<IngredientUpdateDto>(existingEntity);
            patchDoc.ApplyTo(ingredientUpdateDto);

            TryValidateModel(ingredientUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(ingredientUpdateDto, existingEntity);
            var updated = _ingredientRepository.Update(id, existingEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            var ingredientDto = _mapper.Map<IngredientDto>(updated);

            return Ok(_linkService.ExpandSingleIngredientItem(ingredientDto, ingredientDto.Id, version));
        }

        [HttpDelete("{id:int}", Name = nameof(RemoveIngredient))]
        public ActionResult RemoveIngredient(int id)
        {
            var ingredient = _ingredientRepository.GetSingle(id);

            if (ingredient == null)
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
        public ActionResult<IngredientDto> UpdateIngredient(ApiVersion version, int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                return BadRequest();
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

            var ingredientDto = _mapper.Map<IngredientDto>(existingIngredient);

            return Ok(_linkService.ExpandSingleIngredientItem(ingredientDto, ingredientDto.Id, version));
        }

        [HttpGet("GetRandomMeal", Name = nameof(GetRandomMeal))]
        public ActionResult GetRandomMeal()
        {
            var ingredients = _ingredientRepository.GetRandomMeal();
            var dtos = ingredients.Select(x => _mapper.Map<IngredientDto>(x));

            var links = new List<LinkDto>
            {
                new LinkDto(Url.Link(nameof(GetRandomMeal), null), "self", "GET")
            };

            return Ok(new
            {
                value = dtos,
                links = links
            });
        }
    }
}