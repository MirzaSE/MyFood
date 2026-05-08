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
            var entities = _ingredientRepository.GetAll(queryParameters);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(entities));
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await Task.FromResult(_ingredientRepository.Count());
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");
            }

            var entity = _ingredientRepository.GetSingle(id);
            return await Task.FromResult(entity != null ? _mapper.Map<IngredientDto>(entity) : null);
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto ingredientCreateDto)
        {
            if (ingredientCreateDto == null)
            {
                throw new ArgumentNullException(nameof(ingredientCreateDto));
            }

            if (string.IsNullOrWhiteSpace(ingredientCreateDto.Name))
            {
                throw new ArgumentException("Ingredient name is required.", nameof(ingredientCreateDto));
            }

            var existing = _ingredientRepository.SearchByName(ingredientCreateDto.Name)
                .FirstOrDefault(x => x.Name.Equals(ingredientCreateDto.Name, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                throw new InvalidOperationException("Ingredient with the same name already exists.");
            }

            var entity = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            _ingredientRepository.Add(entity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            var newEntity = _ingredientRepository.GetSingle(entity.Id);
            return await Task.FromResult(_mapper.Map<IngredientDto>(newEntity));
        }

        public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto ingredientUpdateDto)
        {
            var existing = _ingredientRepository.GetSingle(id);
            if (existing == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(ingredientUpdateDto.Name))
            {
                existing.Name = ingredientUpdateDto.Name;
            }

            if (!string.IsNullOrWhiteSpace(ingredientUpdateDto.Unit))
            {
                existing.Unit = ingredientUpdateDto.Unit;
            }

            existing.CaloriesPerUnit = ingredientUpdateDto.CaloriesPerUnit;
            existing.Protein = ingredientUpdateDto.Protein;
            existing.Carbs = ingredientUpdateDto.Carbs;
            existing.Fat = ingredientUpdateDto.Fat;

            var updated = _ingredientRepository.Update(id, existing);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return await Task.FromResult(_mapper.Map<IngredientDto>(updated));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = _ingredientRepository.GetSingle(id);
            if (existing == null)
            {
                return false;
            }

            _ingredientRepository.Delete(id);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Deleting an ingredient failed on save.");
            }

            return true;
        }

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string name)
        {
            var results = _ingredientRepository.SearchByName(name ?? string.Empty);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(results));
        }
    }
}
