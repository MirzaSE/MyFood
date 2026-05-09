using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;

namespace MyFood.Api.Controllers.v2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        // GET /api/v2/ingredients
        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
        {
            var dtos = await _ingredientService.GetAllAsync(queryParameters);
            return Ok(new { version = "2.0", value = dtos });
        }

        // GET /api/v2/ingredients/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<IngredientDto>> GetById(int id)
        {
            var dto = await _ingredientService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }
    }
}
