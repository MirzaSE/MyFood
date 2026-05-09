using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFood.Domain.Entities;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Api.Controllers.v1
{
    [Authorize]
    [ApiController]
    [Route("api/ingredients")]
    public class IngredientsController : ControllerBase
    {
        private readonly FoodDbContext _context;

        public IngredientsController(FoodDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var ingredients = _context.Ingredients.ToList();
            return Ok(ingredients);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var ingredient = _context.Ingredients.Find(id);
            if (ingredient == null) return NotFound();
            return Ok(ingredient);
        }

        [HttpPost]
        public IActionResult Create([FromBody] IngredientCreateRequest request)
        {
            var ingredient = new IngredientEntity
            {
                Name = request.Name,
                FoodEntityId = null  // standalone ingredient
            };
            _context.Ingredients.Add(ingredient);
            _context.SaveChanges();
            return Ok(ingredient);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] IngredientCreateRequest request)
        {
            var ingredient = _context.Ingredients.Find(id);
            if (ingredient == null) return NotFound();
            ingredient.Name = request.Name;
            _context.SaveChanges();
            return Ok(ingredient);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var ingredient = _context.Ingredients.Find(id);
            if (ingredient == null) return NotFound();
            _context.Ingredients.Remove(ingredient);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery] string name)
        {
            var results = _context.Ingredients
                .Where(i => i.Name.ToLower().Contains(name.ToLower()))
                .ToList();
            return Ok(results);
        }
    }

    public class IngredientCreateRequest
    {
        public string Name { get; set; } = "";
    }
}