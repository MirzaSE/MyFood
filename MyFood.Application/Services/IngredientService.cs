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
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");
            }

            var ingredientEntity = _ingredientRepository.GetSingle(id);
            return await Task.FromResult(ingredientEntity != null ? _mapper.Map<IngredientDto>(ingredientEntity) : null);
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                throw new ArgumentNullException(nameof(ingredientCreateDto));
            }

            if (string.IsNullOrWhiteSpace(ingredientCreateDto.Name))
            {
                throw new ArgumentNullException(nameof(ingredientCreateDto.Name), "Ingredient name is required.");
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

            var newIngredientEntity = _ingredientRepository.GetSingle(ingredientEntity.Id);
            return await Task.FromResult(_mapper.Map<IngredientDto>(newIngredientEntity));
        }

        public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto ingredientUpdateDto)
        {
            var existingEntity = _ingredientRepository.GetSingle(id);
            if (existingEntity == null)
            {
                return null;
            }

            _mapper.Map(ingredientUpdateDto, existingEntity);
            var updatedEntity = _ingredientRepository.Update(id, existingEntity);

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

            _ingredientRepository.Delete(id);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Deleting an ingredient failed on save.");
            }

            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string name)
        {
            var ingredientEntities = _ingredientRepository.SearchByName(name ?? string.Empty);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(ingredientEntities));
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await Task.FromResult(_ingredientRepository.Count());
        }
    }
}
