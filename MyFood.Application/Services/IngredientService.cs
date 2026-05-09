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

        public Task<IEnumerable<IngredientDto>> GetAllAsync(QueryParameters queryParameters)
        {
            var entities = _repository.GetAll(queryParameters);
            return Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(entities));
        }

        public Task<IEnumerable<IngredientDto>> SearchAsync(string query)
        {
            var entities = string.IsNullOrWhiteSpace(query) ? new List<IngredientEntity>() : _repository.Search(query);
            return Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(entities));
        }

        public Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id < 0) return Task.FromResult<IngredientDto?>(null);
            var entity = _repository.GetById(id);
            return Task.FromResult(entity == null ? null : _mapper.Map<IngredientDto>(entity));
        }

        public Task<IngredientDto> CreateAsync(IngredientCreateDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.Name)) throw new ArgumentException("Name is required");
            if (_repository.Search(createDto.Name).Any(x => x.Name.Equals(createDto.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Duplicate ingredient");

            var entity = _mapper.Map<IngredientEntity>(createDto);
            _repository.Add(entity);
            if (!_repository.Save()) throw new Exception("Creating ingredient failed on save.");
            return Task.FromResult(_mapper.Map<IngredientDto>(entity));
        }

        public Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto updateDto)
        {
            var existing = _repository.GetById(id);
            if (existing == null) return Task.FromResult<IngredientDto?>(null);

            _mapper.Map(updateDto, existing);
            _repository.Update(existing);
            if (!_repository.Save()) throw new Exception("Updating ingredient failed on save.");
            return Task.FromResult<IngredientDto?>(_mapper.Map<IngredientDto>(existing));
        }

        public Task<bool> DeleteAsync(int id)
        {
            var existing = _repository.GetById(id);
            if (existing == null) return Task.FromResult(false);
            _repository.Delete(existing);
            if (!_repository.Save()) throw new Exception("Deleting ingredient failed on save.");
            return Task.FromResult(true);
        }
    }
}
