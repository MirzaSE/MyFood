using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly MyFood.Application.IIngredientRepository _repository;
        private readonly IMapper _mapper;

        public IngredientService(MyFood.Application.IIngredientRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<IngredientDto>> GetAllAsync(QueryParameters queryParameters)
        {
            var entities = await _repository.GetAllAsync(queryParameters);
            return _mapper.Map<IEnumerable<IngredientDto>>(entities);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _repository.CountAsync();
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id <= 0) return null;
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<IngredientDto>(entity);
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.Name))
                throw new ArgumentException("Name is required.");

            if (await _repository.ExistsAsync(createDto.Name))
                throw new InvalidOperationException($"Ingredient '{createDto.Name}' already exists.");

            var entity = _mapper.Map<IngredientEntity>(createDto);
            var created = await _repository.AddAsync(entity);
            return _mapper.Map<IngredientDto>(created);
        }

        public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            if (updateDto.Name != null) existing.Name = updateDto.Name;
            if (updateDto.Unit != null) existing.Unit = updateDto.Unit;
            if (updateDto.CaloriesPerUnit.HasValue) existing.CaloriesPerUnit = updateDto.CaloriesPerUnit.Value;
            if (updateDto.Protein.HasValue) existing.Protein = updateDto.Protein.Value;
            if (updateDto.Carbs.HasValue) existing.Carbs = updateDto.Carbs.Value;
            if (updateDto.Fat.HasValue) existing.Fat = updateDto.Fat.Value;
            if (updateDto.Quantity.HasValue) existing.Quantity = updateDto.Quantity.Value;
            if (updateDto.FoodId.HasValue) existing.FoodEntityId = updateDto.FoodId.Value;

            var updated = await _repository.UpdateAsync(id, existing);
            return updated == null ? null : _mapper.Map<IngredientDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string name)
        {
            var entities = await _repository.SearchAsync(name);
            return _mapper.Map<IEnumerable<IngredientDto>>(entities);
        }
    }
}
