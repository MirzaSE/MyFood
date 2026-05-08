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

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            var entity = _ingredientRepository.GetSingle(id);
            return await Task.FromResult(entity != null ? _mapper.Map<IngredientDto>(entity) : null);
        }

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string query)
        {
            var entities = _ingredientRepository.SearchByName(query);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(entities));
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto createDto)
        {
            var entity = _mapper.Map<IngredientEntity>(createDto);
            _ingredientRepository.Add(entity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            return await Task.FromResult(_mapper.Map<IngredientDto>(entity));
        }

        public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto updateDto)
        {
            var existingEntity = _ingredientRepository.GetSingle(id);
            if (existingEntity == null)
            {
                return null;
            }

            _mapper.Map(updateDto, existingEntity);
            var updatedEntity = _ingredientRepository.Update(id, existingEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return await Task.FromResult(_mapper.Map<IngredientDto>(updatedEntity));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = _ingredientRepository.GetSingle(id);
            if (entity == null)
            {
                return false;
            }

            _ingredientRepository.Delete(id);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Deleting an ingredient failed on save.");
            }

            return await Task.FromResult(true);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await Task.FromResult(_ingredientRepository.Count());
        }
    }
}
