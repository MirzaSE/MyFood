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

        public async Task<int> GetTotalCountAsync(string? query = null)
        {
            return await Task.FromResult(_ingredientRepository.Count(query));
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id < 0)
            {
                return null;
            }

            var entity = _ingredientRepository.GetSingle(id);
            return await Task.FromResult(entity is null ? null : _mapper.Map<IngredientDto>(entity));
        }

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string name)
        {
            var entities = _ingredientRepository.SearchByName(name);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(entities));
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.Name))
            {
                throw new ArgumentException("Ingredient name is required.");
            }

            if (_ingredientRepository.ExistsByName(createDto.Name))
            {
                throw new InvalidOperationException("Ingredient with this name already exists.");
            }

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
            var entity = _ingredientRepository.GetSingle(id);
            if (entity is null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Name) &&
                _ingredientRepository.ExistsByName(updateDto.Name, id))
            {
                throw new InvalidOperationException("Ingredient with this name already exists.");
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Name))
            {
                entity.Name = updateDto.Name;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Unit))
            {
                entity.Unit = updateDto.Unit;
            }

            if (updateDto.CaloriesPerUnit.HasValue)
            {
                entity.CaloriesPerUnit = updateDto.CaloriesPerUnit.Value;
            }

            if (updateDto.Protein.HasValue)
            {
                entity.Protein = updateDto.Protein.Value;
            }

            if (updateDto.Carbs.HasValue)
            {
                entity.Carbs = updateDto.Carbs.Value;
            }

            if (updateDto.Fat.HasValue)
            {
                entity.Fat = updateDto.Fat.Value;
            }

            _ingredientRepository.Update(entity);
            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return await Task.FromResult(_mapper.Map<IngredientDto>(entity));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = _ingredientRepository.GetSingle(id);
            if (entity is null)
            {
                return false;
            }

            _ingredientRepository.Delete(entity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Deleting an ingredient failed on save.");
            }

            return await Task.FromResult(true);
        }
    }
}
