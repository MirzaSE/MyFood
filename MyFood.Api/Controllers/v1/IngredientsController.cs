using Microsoft.AspNetCore.JsonPatch; // Required for PATCH
using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _repository;

        public IngredientsController(IIngredientRepository repository)
        {
            _repository = repository;
        }

        // 1. GET: /api/v1/ingredients - Returns all ingredients
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_repository.GetAll());
        }

        // 2. GET: /api/v1/ingredients/{id}
        [HttpGet("{id}", Name = "GetIngredient")]
        public IActionResult GetById(int id)
        {
            var item = _repository.GetSingle(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // 3. POST: /api/v1/ingredients
        [HttpPost]
        public IActionResult Create([FromBody] IngredientEntity ingredient)
        {
            _repository.Add(ingredient);
            if (!_repository.Save()) return BadRequest();
            return CreatedAtRoute("GetIngredient", new { id = ingredient.Id }, ingredient);
        }

        // 4. PUT: /api/v1/ingredients/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] IngredientEntity ingredient)
        {
            var existing = _repository.GetSingle(id);
            if (existing == null) return NotFound();

            ingredient.Id = id;
            _repository.Update(ingredient);
            _repository.Save();
            return NoContent();
        }

        // 5. PATCH: /api/v1/ingredients/{id} (Missing in your screenshot)
        [HttpPatch("{id}")]
        public IActionResult PartialUpdate(int id, [FromBody] JsonPatchDocument<IngredientEntity> patchDoc)
        {
            var entity = _repository.GetSingle(id);
            if (entity == null) return NotFound();

            patchDoc.ApplyTo(entity, ModelState);
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _repository.Save();
            return NoContent();
        }

        // 6. DELETE: /api/v1/ingredients/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_repository.GetSingle(id) == null) return NotFound();
            _repository.Delete(id);
            _repository.Save();
            return NoContent();
        }

        // 7. GET: /api/v1/ingredients/search (Missing in your screenshot)
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string name)
        {
            return Ok(_repository.Search(name ?? ""));
        }
    }
}