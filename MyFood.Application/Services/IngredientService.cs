using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _repository;
        private readonly IMapper _mapper;

        public IngredientService(IIngredientRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<IngredientDto>> GetAllAsync(QueryParameters? parameters = null)
        {
            parameters ??= new QueryParameters();
            var entities = _repository.GetAll(parameters);
            return _mapper.Map<IEnumerable<IngredientDto>>(entities);
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            var entity = _repository.GetSingle(id);
            if (entity == null)
                return null;

            return _mapper.Map<IngredientDto>(entity);
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto ingredientCreateDto)
        {
            if (string.IsNullOrWhiteSpace(ingredientCreateDto.Name))
                throw new ArgumentException("Ingredient name cannot be empty");

            if (string.IsNullOrWhiteSpace(ingredientCreateDto.Unit))
                throw new ArgumentException("Ingredient unit cannot be empty");

            var existing = await _repository.GetByNameAsync(ingredientCreateDto.Name);
            if (existing != null)
                throw new InvalidOperationException($"Ingredient with name '{ingredientCreateDto.Name}' already exists");

            var entity = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            var created = await _repository.CreateAsync(entity);
            return _mapper.Map<IngredientDto>(created);
        }

        public async Task<IngredientDto> UpdateAsync(int id, IngredientUpdateDto ingredientUpdateDto)
        {
            var entity = _repository.GetSingle(id);
            if (entity == null)
                throw new InvalidOperationException($"Ingredient with id {id} not found");

            if (!string.IsNullOrWhiteSpace(ingredientUpdateDto.Name))
            {
                var existing = await _repository.GetByNameAsync(ingredientUpdateDto.Name);
                if (existing != null && existing.Id != id)
                    throw new InvalidOperationException($"Ingredient with name '{ingredientUpdateDto.Name}' already exists");
                entity.Name = ingredientUpdateDto.Name;
            }

            if (!string.IsNullOrWhiteSpace(ingredientUpdateDto.Unit))
                entity.Unit = ingredientUpdateDto.Unit;

            if (ingredientUpdateDto.CaloriesPerUnit.HasValue)
                entity.CaloriesPerUnit = ingredientUpdateDto.CaloriesPerUnit.Value;

            if (ingredientUpdateDto.Protein.HasValue)
                entity.Protein = ingredientUpdateDto.Protein.Value;

            if (ingredientUpdateDto.Carbs.HasValue)
                entity.Carbs = ingredientUpdateDto.Carbs.Value;

            if (ingredientUpdateDto.Fat.HasValue)
                entity.Fat = ingredientUpdateDto.Fat.Value;

            var updated = await _repository.UpdateAsync(entity);
            return _mapper.Map<IngredientDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<IngredientDto>();

            var entities = await _repository.SearchAsync(searchTerm);
            return _mapper.Map<IEnumerable<IngredientDto>>(entities);
        }
    }
}
