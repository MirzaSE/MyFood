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

    public class IngredientController : ControllerBase
    {

        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;
        private readonly ILinkService<IngredientController> _linkService;

        public IngredientController(
            IIngredientRepository IngredientRepository,
            IMapper mapper,
            ILinkService<IngredientController> linkService)
        {
            _ingredientRepository = IngredientRepository;
            _mapper = mapper;
            _linkService = linkService;
        }

        [HttpGet(Name = nameof(GetAllIngredient))]
        public ActionResult GetAllIngredient(ApiVersion version, [FromQuery] QueryParameters queryParameters)
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
            IngredientEntity foodItem = _ingredientRepository.GetSingle(id);

            if (foodItem == null)
            {
                return NotFound();
            }

            FoodDto item = _mapper.Map<FoodDto>(foodItem);

            return Ok(_linkService.ExpandSingleIngredientItem(item, item.Id, version));
        }

       

        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult<IngredientDto> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto IngredientCreateDto)
        {
            if (IngredientCreateDto == null)
            {
                return BadRequest();
            }

            IngredientEntity toAdd = _mapper.Map<IngredientEntity>(IngredientCreateDto);

            _ingredientRepository.Add(toAdd);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating a fooditem failed on save.");
            }

            IngredientEntity newIngredientItem = _ingredientRepository.GetSingle(toAdd.Id);
            IngredientDto ingredientDto = _mapper.Map<IngredientDto>(newIngredientItem);

            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { version = version.ToString(), id = newIngredientItem.Id },
                _linkService.ExpandSingleIngredientItem(ingredientDto, ingredientDto.Id, version));
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
                throw new Exception("Updating a ingredientitem failed on save.");
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
                throw new Exception("Deleting a Ingredientitem failed on save.");
            }

            return NoContent();
        }

    }
}

