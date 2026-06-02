using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public IngredientService(IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<IngredientDto>> GetAllAsync(QueryParameters queryParameters)
        {
            var ingredientEntities = _ingredientRepository.GetAll(queryParameters);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(ingredientEntities));
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id < 1)
            {
                return null;
            }

            var ingredientEntity = _ingredientRepository.GetSingle(id);
            return await Task.FromResult(ingredientEntity != null ? _mapper.Map<IngredientDto>(ingredientEntity) : null);
        }

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return await Task.FromResult(Enumerable.Empty<IngredientDto>());
            }

            var ingredientEntities = _ingredientRepository.SearchByName(name);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(ingredientEntities));
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                throw new ArgumentNullException(nameof(ingredientCreateDto));
            }

            if (string.IsNullOrWhiteSpace(ingredientCreateDto.Name))
            {
                throw new ArgumentException("Ingredient name is required.", nameof(ingredientCreateDto.Name));
            }

            if (_ingredientRepository.ExistsByName(ingredientCreateDto.Name))
            {
                throw new InvalidOperationException($"Ingredient with name '{ingredientCreateDto.Name}' already exists.");
            }

            var ingredientEntity = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            _ingredientRepository.Add(ingredientEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            var newIngredient = _ingredientRepository.GetSingle(ingredientEntity.Id);
            return await Task.FromResult(_mapper.Map<IngredientDto>(newIngredient));
        }

        public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto ingredientUpdateDto)
        {
            if (ingredientUpdateDto == null)
            {
                throw new ArgumentNullException(nameof(ingredientUpdateDto));
            }

            var existingEntity = _ingredientRepository.GetSingle(id);
            if (existingEntity == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(ingredientUpdateDto.Name))
            {
                if (!string.Equals(existingEntity.Name, ingredientUpdateDto.Name, StringComparison.OrdinalIgnoreCase)
                    && _ingredientRepository.ExistsByName(ingredientUpdateDto.Name))
                {
                    throw new InvalidOperationException($"Ingredient with name '{ingredientUpdateDto.Name}' already exists.");
                }

                existingEntity.Name = ingredientUpdateDto.Name;
            }

            if (!string.IsNullOrWhiteSpace(ingredientUpdateDto.Unit))
            {
                existingEntity.Unit = ingredientUpdateDto.Unit;
            }

            if (ingredientUpdateDto.CaloriesPerUnit.HasValue)
            {
                existingEntity.CaloriesPerUnit = ingredientUpdateDto.CaloriesPerUnit.Value;
            }

            if (ingredientUpdateDto.Protein.HasValue)
            {
                existingEntity.Protein = ingredientUpdateDto.Protein.Value;
            }

            if (ingredientUpdateDto.Carbs.HasValue)
            {
                existingEntity.Carbs = ingredientUpdateDto.Carbs.Value;
            }

            if (ingredientUpdateDto.Fat.HasValue)
            {
                existingEntity.Fat = ingredientUpdateDto.Fat.Value;
            }

            var updatedEntity = _ingredientRepository.Update(existingEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return await Task.FromResult(_mapper.Map<IngredientDto>(updatedEntity));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ingredientEntity = _ingredientRepository.GetSingle(id);
            if (ingredientEntity == null)
            {
                return false;
            }

            _ingredientRepository.Delete(ingredientEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Deleting an ingredient failed on save.");
            }

            return await Task.FromResult(true);
        }
    }
}
