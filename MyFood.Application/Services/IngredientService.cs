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

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto createDto)
        {
            var entity = _mapper.Map<IngredientEntity>(createDto);
            _repo.Add(entity);
            if (!_repo.Save()) throw new Exception("Failed to save ingredient");
            var newEntity = _repo.GetSingle(entity.Id);
            return await Task.FromResult(_mapper.Map<IngredientDto>(newEntity));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = _repo.GetSingle(id);
            if (existing == null) return await Task.FromResult(false);
            _repo.Delete(id);
            if (!_repo.Save()) throw new Exception("Failed to delete ingredient");
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<IngredientDto>> GetAllAsync(QueryParameters queryParameters)
        {
            var entities = _repo.GetAll(queryParameters);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(entities));
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            var e = _repo.GetSingle(id);
            return await Task.FromResult(e != null ? _mapper.Map<IngredientDto>(e) : null);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await Task.FromResult(_repo.Count());
        }

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string name)
        {
            var items = _repo.SearchByName(name);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(items));
        }

        public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto updateDto)
        {
            var existing = _repo.GetSingle(id);
            if (existing == null) return await Task.FromResult<IngredientDto?>(null);

            _mapper.Map(updateDto, existing);
            var updated = _repo.Update(id, existing);
            if (!_repo.Save()) throw new Exception("Failed to update ingredient");
            return await Task.FromResult(_mapper.Map<IngredientDto>(updated));
        }
    }
}
