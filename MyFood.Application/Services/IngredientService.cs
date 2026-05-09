using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _repo;
        private readonly IMapper _mapper;

        public IngredientService(IIngredientRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public Task<IEnumerable<IngredientDto>> GetAllAsync(QueryParameters queryParameters)
        {
            var entities = _repo.GetAll(queryParameters).ToList();
            return Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(entities));
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");

            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<IngredientDto>(entity);
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.", nameof(dto));

            if (await _repo.ExistsByNameAsync(dto.Name))
                throw new InvalidOperationException($"An ingredient named '{dto.Name}' already exists.");

            var entity = _mapper.Map<IngredientEntity>(dto);
            var created = await _repo.AddAsync(entity);
            return _mapper.Map<IngredientDto>(created);
        }

        public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            // AutoMapper profile is configured to skip nulls -> partial update
            _mapper.Map(dto, existing);

            var updated = await _repo.UpdateAsync(id, existing);
            return updated == null ? null : _mapper.Map<IngredientDto>(updated);
        }

        public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string name)
        {
            // Repository handles case-insensitive matching via SQL LIKE (collation-dependent)
            // Fallback to in-memory ToLower comparison to guarantee case-insensitivity.
            var entities = await _repo.SearchAsync(name ?? string.Empty);

            if (!string.IsNullOrWhiteSpace(name))
            {
                var needle = name.ToLowerInvariant();
                entities = entities.Where(x => x.Name != null && x.Name.ToLowerInvariant().Contains(needle));
            }

            return _mapper.Map<IEnumerable<IngredientDto>>(entities);
        }

        public Task<int> GetTotalCountAsync() => _repo.CountAsync();
    }
}
