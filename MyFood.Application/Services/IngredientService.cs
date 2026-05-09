using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Application.Services.Interfaces;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public interface IIngredientRepository
    {
        IngredientEntity GetSingle(int id);
        void Add(IngredientEntity item);
        void Delete(int id);
        IngredientEntity Update(int id, IngredientEntity item);
        IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters);
        IEnumerable<IngredientEntity> SearchFoodsByName(string name);
        int Count();
        bool Save();
    }

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
            var entities = _repo.GetAll(queryParameters);
            return Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(entities));
        }

        public Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id < 0) return Task.FromResult<IngredientDto?>(null);
            var entity = _repo.GetSingle(id);
            return Task.FromResult(_mapper.Map<IngredientDto?>(entity));
        }

        public Task<IngredientDto> CreateAsync(IngredientCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.");

            var entity = _mapper.Map<IngredientEntity>(dto);
            _repo.Add(entity);
            _repo.Save();
            return Task.FromResult(_mapper.Map<IngredientDto>(entity));
        }

        public Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto dto)
        {
            var entity = _repo.GetSingle(id);
            if (entity == null) return Task.FromResult<IngredientDto?>(null);

            if (dto.Name != null) entity.Name = dto.Name;
            if (dto.Unit != null) entity.Unit = dto.Unit;
            if (dto.CaloriesPerUnit.HasValue) entity.CaloriesPerUnit = dto.CaloriesPerUnit.Value;
            if (dto.Protein.HasValue) entity.Protein = dto.Protein.Value;
            if (dto.Carbs.HasValue) entity.Carbs = dto.Carbs.Value;
            if (dto.Fat.HasValue) entity.Fat = dto.Fat.Value;

            _repo.Update(id, entity);
            _repo.Save();
            return Task.FromResult<IngredientDto?>(_mapper.Map<IngredientDto>(entity));
        }

        public Task<bool> DeleteAsync(int id)
        {
            var entity = _repo.GetSingle(id);
            if (entity == null) return Task.FromResult(false);

            _repo.Delete(id);
            _repo.Save();
            return Task.FromResult(true);
        }

        public Task<IEnumerable<IngredientDto>> SearchAsync(string name)
        {
            var results = _repo.SearchFoodsByName(name);
            return Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(results));
        }
    }
}