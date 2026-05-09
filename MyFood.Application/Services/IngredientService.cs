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

        public async Task<IEnumerable<IngredientDto>> GetAllAsync(QueryParameters queryParameters)
        {
            var entities = await _repo.GetAll(queryParameters);
            return _mapper.Map<IEnumerable<IngredientDto>>(entities);
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be non-negative.");

            var entity = await _repo.GetSingle(id);
            return entity == null ? null : _mapper.Map<IngredientDto>(entity);
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.", nameof(dto.Name));

            var existing = await _repo.GetAll(new QueryParameters { PageCount = 50 });
            if (existing.Any(x => x.Name != null && x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Ingredient '{dto.Name}' already exists.");

            var entity = _mapper.Map<IngredientEntity>(dto);
            entity.Created = DateTime.UtcNow;
            var added = await _repo.Add(entity);
            return _mapper.Map<IngredientDto>(added);
        }

        public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto dto)
        {
            var existing = await _repo.GetSingle(id);
            if (existing == null)
                return null;

            if (dto.Name != null)
                existing.Name = dto.Name;

            var updated = await _repo.Update(id, existing);
            return _mapper.Map<IngredientDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _repo.Delete(id);
            return deleted != null;
        }

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string query)
        {
            var all = await _repo.GetAll(new QueryParameters { PageCount = 50 });
            var filtered = all.Where(x => x.Name != null &&
                x.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
            return _mapper.Map<IEnumerable<IngredientDto>>(filtered);
        }

        public async Task<int> GetCountAsync()
        {
            return await _repo.Count();
        }
    }
}
