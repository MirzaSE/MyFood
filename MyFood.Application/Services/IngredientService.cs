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

        public async Task<IEnumerable<IngridientDto>> GetAllAsync(QueryParameters queryParameters)
        {
            var entities = _repository.GetAll(queryParameters);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngridientDto>>(entities));
        }

        public async Task<IngridientDto?> GetByIdAsync(int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");
            var entity = _repository.GetSingle(id);
            return await Task.FromResult(entity != null ? _mapper.Map<IngridientDto>(entity) : null);
        }

        public async Task<IngridientDto> CreateAsync(IngridientCreateDto createDto)
        {
            if (string.IsNullOrEmpty(createDto.Name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(createDto));

            var duplicates = _repository.SearchFoodsByName(createDto.Name);
            if (duplicates.Any(i => i.Name.Equals(createDto.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("An ingredient with this name already exists.");

            var entity = _mapper.Map<IngredientEntity>(createDto);
            _repository.Add(entity);

            if (!_repository.Save())
                throw new Exception("Creating an ingredient failed on save.");

            var newEntity = _repository.GetSingle(entity.Id);
            return await Task.FromResult(_mapper.Map<IngridientDto>(newEntity!));
        }

        public async Task<IngridientDto?> UpdateAsync(int id, IngridientUpdate updateDto)
        {
            var entity = _repository.GetSingle(id);
            if (entity == null) return null;

            _mapper.Map(updateDto, entity);
            var updated = _repository.Update(id, entity);

            if (!_repository.Save())
                throw new Exception("Updating an ingredient failed on save.");

            return await Task.FromResult(_mapper.Map<IngridientDto>(updated));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = _repository.GetSingle(id);
            if (entity == null) return false;

            _repository.Delete(id);

            if (!_repository.Save())
                throw new Exception("Deleting an ingredient failed on save.");

            return true;
        }

        public async Task<IEnumerable<IngridientDto>> SearchAsync(string name)
        {
            var entities = _repository.SearchFoodsByName(name);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngridientDto>>(entities));
        }
    }
}
