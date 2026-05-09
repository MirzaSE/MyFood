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

        public async Task<IEnumerable<IngredientDto>> GetAllAsync(QueryParameters queryParameters)
        {
            var entities = _repository.GetAll(queryParameters);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(entities));
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");
            }

            var entity = _repository.GetSingle(id);
            return await Task.FromResult(entity != null ? _mapper.Map<IngredientDto>(entity) : null);
        }

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string name)
        {
            var entities = _repository.SearchByName(name ?? string.Empty);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(entities));
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto createDto)
        {
            if (createDto == null)
            {
                throw new ArgumentNullException(nameof(createDto));
            }
            if (string.IsNullOrWhiteSpace(createDto.Name))
            {
                throw new ArgumentException("Ingredient name is required.", nameof(createDto));
            }
            if (_repository.ExistsByName(createDto.Name))
            {
                throw new InvalidOperationException($"An ingredient named '{createDto.Name}' already exists.");
            }

            var entity = _mapper.Map<IngredientEntity>(createDto);
            _repository.Add(entity);

            if (!_repository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            var saved = _repository.GetSingle(entity.Id) ?? entity;
            return await Task.FromResult(_mapper.Map<IngredientDto>(saved));
        }

        public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto updateDto)
        {
            var existing = _repository.GetSingle(id);
            if (existing == null)
            {
                return null;
            }

            // Partial update — AutoMapper config skips null members.
            _mapper.Map(updateDto, existing);
            var updated = _repository.Update(id, existing);

            if (!_repository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return await Task.FromResult(_mapper.Map<IngredientDto>(updated));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = _repository.GetSingle(id);
            if (existing == null)
            {
                return false;
            }

            _repository.Delete(id);

            if (!_repository.Save())
            {
                throw new Exception("Deleting an ingredient failed on save.");
            }

            return await Task.FromResult(true);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await Task.FromResult(_repository.Count());
        }
    }
}
