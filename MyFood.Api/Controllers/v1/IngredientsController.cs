using Microsoft.AspNetCore.Mvc;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Services;

namespace MyFood.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class IngredientsController : ControllerBase
{
    private readonly IIngredientService _ingredientService;
    private readonly IFoodRepository _foodRepository;

    public IngredientsController(IIngredientService ingredientService, IFoodRepository foodRepository)
    {
        _ingredientService = ingredientService;
        _foodRepository = foodRepository;
    }

    [HttpGet(Name = nameof(GetAllIngredients))]
    public async Task<ActionResult<IEnumerable<IngredientDto>>> GetAllIngredients([FromQuery] QueryParameters queryParameters)
    {
        var dtos = await _ingredientService.GetAllAsync(queryParameters);

        return Ok(dtos);
    }

    [HttpGet("{id:int}", Name = nameof(GetSingleIngredient))]
    public async Task<ActionResult<IngredientDto>> GetSingleIngredient(int id)
    {
        var dto = await _ingredientService.GetByIdAsync(id);

        if (dto == null)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    [HttpGet("search", Name = nameof(SearchIngredients))]
    public async Task<ActionResult<IEnumerable<IngredientDto>>> SearchIngredients([FromQuery] string name)
    {
        var dtos = await _ingredientService.SearchAsync(name);

        return Ok(dtos);
    }

    [HttpPost(Name = nameof(AddIngredient))]
    public async Task<ActionResult<IngredientDto>> AddIngredient(ApiVersion version, [FromBody] IngredientCreateDto ingredientCreateDto)
    {
        if (ingredientCreateDto == null) return BadRequest();
        if (ingredientCreateDto.FoodEntityId.HasValue && _foodRepository.GetSingle(ingredientCreateDto.FoodEntityId.Value) == null)
        {
            return NotFound("Food item was not found.");
        }

        try
        {
            var dtoToReturn = await _ingredientService.CreateAsync(ingredientCreateDto);

            return CreatedAtRoute(nameof(GetSingleIngredient),
                new { version = version.ToString(), id = dtoToReturn.Id },
                dtoToReturn);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id:int}", Name = nameof(UpdateIngredient))]
    public async Task<ActionResult<IngredientDto>> UpdateIngredient(int id, [FromBody] IngredientUpdateDto ingredientUpdateDto)
    {
        if (ingredientUpdateDto == null) return BadRequest();

        try
        {
            var dto = await _ingredientService.UpdateAsync(id, ingredientUpdateDto);
            if (dto == null) return NotFound();

            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id:int}", Name = nameof(RemoveIngredient))]
    public async Task<ActionResult> RemoveIngredient(int id)
    {
        var deleted = await _ingredientService.DeleteAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}
