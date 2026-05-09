using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Helpers;
using MyFood.Infrastructure.Repositories;
using MyFood.Application.Services;
using System.Text.Json;

namespace MyFood.Api.Controllers.v1
{
    //[Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IMapper _mapper;

        public IngredientsController(
            IIngredientRepository ingredientRepository,
            IFoodRepository foodRepository,
            IMapper mapper
            )
        {
            _ingredientRepository = ingredientRepository;
            _foodRepository = foodRepository;
            _mapper = mapper;
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

            var ingredientDto = _mapper.Map<IngredientDto>(ingredientEntity);

            return Ok(ingredientDto);
        }

        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult AddIngredient(IngredientCreateDto ingredientCreateDto)
        {
            var food = _foodRepository.GetSingle(ingredientCreateDto.FoodEntityId);
            if (food == null)
            {
                return NotFound("FoodEntity Not Found");
            }

            var ingredientEntity = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            ingredientEntity.FoodEntity = food;

            _ingredientRepository.Add(ingredientEntity);
            if (!_foodRepository.Save())
            {
                throw new Exception("Creating a ingredient failed on save.");
            }

            var ingredientDto = _mapper.Map<IngredientDto>(ingredientEntity);

            return Ok(ingredientDto);
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

            var ingredientDtos = _mapper.Map<IEnumerable<IngredientDto>>(ingredientItems);

            return Ok(ingredientDtos);
        }

        [HttpGet]
        [Route("by-food/{foodId:int}", Name = nameof(GetIngredientsByFood))]
        public ActionResult GetIngredientsByFood(int foodId)
        {
            var food = _foodRepository.GetSingle(foodId);
            if (food == null)
            {
                return NotFound("FoodEntity Not Found");
            }

            var ingredients = _ingredientRepository.GetAll(new QueryParameters()).Where(i => i.FoodEntity.Id == foodId).ToList();
            var ingredientDtos = _mapper.Map<IEnumerable<IngredientDto>>(ingredients);

            return Ok(ingredientDtos);
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult<IngredientDto> UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
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

            IngredientDto ingredientDto = _mapper.Map<IngredientDto>(existingIngredient);

            return Ok(ingredientDto);
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateIngredient))]
        public ActionResult<IngredientDto> PartiallyUpdateIngredient(int id, [FromBody] JsonPatchDocument<IngredientUpdateDto> patchDoc)
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
                throw new Exception("Updating an ingredient failed on save.");
            }

            IngredientDto ingredientDto = _mapper.Map<IngredientDto>(updated);

            return Ok(ingredientDto);
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(DeleteIngredient))]
        public ActionResult DeleteIngredient(int id)
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
    }
}
