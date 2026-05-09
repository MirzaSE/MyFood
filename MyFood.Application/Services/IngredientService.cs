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

        public async Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync(QueryParameters queryParameters)
        {
            var ingredientEntities = _ingredientRepository.GetAll(queryParameters);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(ingredientEntities));
        }

        public async Task<IngredientDto?> GetIngredientByIdAsync(int id)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");
            }

            var ingredientEntity = _ingredientRepository.GetSingle(id);
            return await Task.FromResult(ingredientEntity != null ? _mapper.Map<IngredientDto>(ingredientEntity) : null);
        }

        public async Task<IEnumerable<IngredientDto>> SearchIngredientsByNameAsync(string name)
        {
            var ingredientEntities = _ingredientRepository.SearchIngredientsByName(name);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(ingredientEntities));
        }

        public async Task<IngredientDto> CreateIngredientAsync(IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                throw new ArgumentNullException(nameof(ingredientCreateDto));
            }

            if (string.IsNullOrWhiteSpace(ingredientCreateDto.Name))
            {
                throw new ArgumentException("Ingredient name is required.", nameof(ingredientCreateDto));
            }

            var existingIngredients = _ingredientRepository.SearchIngredientsByName(ingredientCreateDto.Name);
            if (existingIngredients.Any(x => string.Equals(x.Name, ingredientCreateDto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("An ingredient with the same name already exists.");
            }

            var ingredientEntity = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            _ingredientRepository.Add(ingredientEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient item failed on save.");
            }

            var newIngredientEntity = _ingredientRepository.GetSingle(ingredientEntity.Id);
            return await Task.FromResult(_mapper.Map<IngredientDto>(newIngredientEntity));
        }

        public async Task<IngredientDto?> UpdateIngredientAsync(int id, IngredientUpdateDto ingredientUpdateDto)
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
                existingEntity.Name = ingredientUpdateDto.Name;
            }

            existingEntity.Quantity = ingredientUpdateDto.Quantity;
            _mapper.Map(ingredientUpdateDto, existingEntity);
            var updatedEntity = _ingredientRepository.Update(id, existingEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient item failed on save.");
            }

            return await Task.FromResult(_mapper.Map<IngredientDto>(updatedEntity));
        }

        public async Task<bool> DeleteIngredientAsync(int id)
        {
            var ingredientEntity = _ingredientRepository.GetSingle(id);
            if (ingredientEntity == null)
            {
                return false;
            }

            _ingredientRepository.Delete(id);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Deleting an ingredient item failed on save.");
            }

            return await Task.FromResult(true);
        }

        public async Task<int> GetTotalIngredientCountAsync()
        {
            return await Task.FromResult(_ingredientRepository.Count());
        }
    }
}
