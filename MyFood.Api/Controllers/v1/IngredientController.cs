using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Helpers;
using MyFood.Infrastructure.Repositories;


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
            IIngredientRepository ingredientRepository,
            IMapper mapper,
            ILinkService<IngredientController> linkService)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
            _linkService = linkService;
        }
        // GET ALL
        [HttpGet(Name = nameof(GetAllIngredients))]
        public ActionResult GetAllIngredients(ApiVersion version, [FromQuery] QueryParameters queryParameters)
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

            var result = _mapper.Map<List<IngredientDto>>(ingredientItems);

            return Ok(result);
        }

        
        // GET SINGLE
        [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
        public ActionResult GetSingleIngredient(ApiVersion version, int id)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id),
                    "ID must be non-negative.");
            }

            IngredientEntity ingredientItem =
                _ingredientRepository.GetSingle(id);

            if (ingredientItem == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<IngredientDto>(ingredientItem);

            return Ok(result);
        }

        // POST
        [HttpPost(Name = nameof(AddIngredient))]
        public ActionResult AddIngredient([FromBody] IngredientCreateDto ingredient)
        {
            if (ingredient == null)
            {
                return BadRequest();
            }
            IngredientEntity toAdd = _mapper.Map<IngredientEntity>(ingredient);

            _ingredientRepository.Add(toAdd);
            
            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating ingredient failed on save.");
            }

            return Ok(_mapper.Map<IngredientDto>(toAdd));
        }

        // PUT
        [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
        public ActionResult UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientDto)
        {
            if (ingredientDto == null)
            {
                return BadRequest();
            }

            var existing = _ingredientRepository.GetSingle(id);

            if (existing == null)
            {
                return NotFound();
            }

            _mapper.Map(ingredientDto, existing);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating ingredient failed.");
            }

            return Ok(_mapper.Map<IngredientDto>(existing));
        }

        // DELETE
        [HttpDelete("{id:int}", Name = nameof(DeleteIngredient))]
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
                throw new Exception("Deleting a ingredient failed on save.");
            }

            return NoContent();
        }
    }
}