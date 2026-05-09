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
            var parameters = queryParameters ?? new QueryParameters();
            var page = parameters.Page < 1 ? 1 : parameters.Page;
            var pageCount = parameters.PageCount < 1 ? 1 : parameters.PageCount;

            var items = _ingredientRepository
                .GetAll()
                .OrderBy(i => i.Name)
                .Skip((page - 1) * pageCount)
                .Take(pageCount)
                .ToList();

            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(items));
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id < 0)
            {
                return await Task.FromResult<IngredientDto?>(null);
            }

            var entity = _ingredientRepository.GetSingle(id);
            return await Task.FromResult(entity == null ? null : _mapper.Map<IngredientDto>(entity));
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto createDto)
        {
            if (createDto == null || string.IsNullOrWhiteSpace(createDto.Name))
            {
                throw new ArgumentException("Ingredient name is required.", nameof(createDto.Name));
            }

            var duplicate = _ingredientRepository
                .GetAll()
                .Any(i => i.Name != null && i.Name.Equals(createDto.Name, StringComparison.OrdinalIgnoreCase));

            if (duplicate)
            {
                throw new InvalidOperationException("Ingredient already exists.");
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
            var existingEntity = _ingredientRepository.GetSingle(id);
            if (existingEntity == null || updateDto == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Name))
            {
                existingEntity.Name = updateDto.Name;
            }

            if (updateDto.Unit != null)
            {
                existingEntity.Unit = updateDto.Unit;
            }

            if (updateDto.CaloriesPerUnit.HasValue)
            {
                existingEntity.CaloriesPerUnit = updateDto.CaloriesPerUnit;
            }

            if (updateDto.Protein.HasValue)
            {
                existingEntity.Protein = updateDto.Protein;
            }

            if (updateDto.Carbs.HasValue)
            {
                existingEntity.Carbs = updateDto.Carbs;
            }

            if (updateDto.Fat.HasValue)
            {
                existingEntity.Fat = updateDto.Fat;
            }

            if (updateDto.FoodId.HasValue)
            {
                existingEntity.FoodId = updateDto.FoodId.Value;
            }

            var updatedEntity = _ingredientRepository.Update(existingEntity);

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

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return await Task.FromResult(Enumerable.Empty<IngredientDto>());
            }

            var matches = _ingredientRepository
                .GetAll()
                .Where(i => i.Name != null && i.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(matches));
        }
    }
}
