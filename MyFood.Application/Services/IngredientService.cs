using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _repository;

        public IngredientService(IIngredientRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<IngredientDto>> GetAllAsync(int page = 1, int pageCount = 10)
        {
            var ingredients = _repository.GetAll()
                .Skip(pageCount * (page - 1))
                .Take(pageCount)
                .Select(MapToDto);

            return await Task.FromResult(ingredients);
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id < 0)
                return null;

            var entity = _repository.GetSingle(id);

            return await Task.FromResult(entity == null ? null : MapToDto(entity));
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Ingredient name is required.");

            if (_repository.GetAll().Any(i => i.Name.ToLower() == dto.Name.ToLower()))
                throw new InvalidOperationException("Ingredient already exists.");

            var entity = new IngredientEntity
            {
                Name = dto.Name,
                Unit = dto.Unit,
                CaloriesPerUnit = dto.CaloriesPerUnit,
                Protein = dto.Protein,
                Carbs = dto.Carbs,
                Fat = dto.Fat
            };

            _repository.Add(entity);

            if (!_repository.Save())
                throw new Exception("Failed to save ingredient.");

            return await Task.FromResult(MapToDto(entity));
        }

        public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto dto)
        {
            var entity = _repository.GetSingle(id);

            if (entity == null)
                return null;

            if (!string.IsNullOrWhiteSpace(dto.Name))
                entity.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Unit))
                entity.Unit = dto.Unit;

            if (dto.CaloriesPerUnit.HasValue)
                entity.CaloriesPerUnit = dto.CaloriesPerUnit.Value;

            if (dto.Protein.HasValue)
                entity.Protein = dto.Protein.Value;

            if (dto.Carbs.HasValue)
                entity.Carbs = dto.Carbs.Value;

            if (dto.Fat.HasValue)
                entity.Fat = dto.Fat.Value;

            _repository.Update(id, entity);

            if (!_repository.Save())
                throw new Exception("Failed to update ingredient.");

            return await Task.FromResult(MapToDto(entity));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = _repository.GetSingle(id);

            if (entity == null)
                return false;

            _repository.Delete(id);

            return await Task.FromResult(_repository.Save());
        }

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string term)
        {
            var result = _repository.GetAll()
                .Where(i => i.Name.ToLower().Contains(term.ToLower()))
                .Select(MapToDto);

            return await Task.FromResult(result);
        }

        private static IngredientDto MapToDto(IngredientEntity entity)
        {
            return new IngredientDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Unit = entity.Unit,
                CaloriesPerUnit = entity.CaloriesPerUnit,
                Protein = entity.Protein,
                Carbs = entity.Carbs,
                Fat = entity.Fat
            };
        }
    }
}