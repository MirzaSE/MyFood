using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
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

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id < 0) return null;
            var entity = _ingredientRepository.GetSingle(id);
            return await Task.FromResult(entity != null ? _mapper.Map<IngredientDto>(entity) : null);
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.");
            if (string.IsNullOrWhiteSpace(dto.Unit))
                throw new ArgumentException("Unit is required.");

            var entity = _mapper.Map<IngredientEntity>(dto);
            _ingredientRepository.Add(entity);

            if (!_ingredientRepository.Save())
                throw new Exception("Creating ingredient failed on save.");

            return await Task.FromResult(_mapper.Map<IngredientDto>(entity));
        }

        public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto dto)
        {
            var existing = _ingredientRepository.GetSingle(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            _ingredientRepository.Update(id, existing);

            if (!_ingredientRepository.Save())
                throw new Exception("Updating ingredient failed on save.");

            return await Task.FromResult(_mapper.Map<IngredientDto>(existing));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = _ingredientRepository.GetSingle(id);
            if (entity == null) return false;

            _ingredientRepository.Delete(id);

            if (!_ingredientRepository.Save())
                throw new Exception("Deleting ingredient failed on save.");

            return true;
        }

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string name)
        {
            var entities = _ingredientRepository.SearchIngredientByName(name);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(entities));
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await Task.FromResult(_ingredientRepository.Count());
        }
    }
}
