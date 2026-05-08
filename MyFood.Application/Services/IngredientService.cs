using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IMapper _mapper;

        public IngredientService(
            IIngredientRepository ingredientRepository,
            IFoodRepository foodRepository,
            IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _foodRepository = foodRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<IngredientDto>> GetAllAsync(QueryParameters queryParameters)
        {
            var entities = _ingredientRepository.GetAll(queryParameters);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(entities));
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id < 0)
            {
                return null;
            }

            var entity = _ingredientRepository.GetSingle(id);
            if (entity == null)
            {
                return null;
            }

            return await Task.FromResult(_mapper.Map<IngredientDto>(entity));
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.Name))
            {
                throw new ArgumentException("Ingredient name is required.", nameof(createDto));
            }

            if (_foodRepository.GetSingle(createDto.FoodEntityId) == null)
            {
                throw new ArgumentException("Referenced food does not exist.", nameof(createDto));
            }

            var possibleDuplicates = _ingredientRepository.GetAll(new QueryParameters
            {
                Query = createDto.Name,
                Page = 1,
                PageCount = 50
            });

            var duplicateExists = possibleDuplicates.Any(x =>
                x.FoodEntityId == createDto.FoodEntityId &&
                string.Equals(x.Name, createDto.Name, StringComparison.OrdinalIgnoreCase));

            if (duplicateExists)
            {
                throw new InvalidOperationException("Ingredient with the same name already exists for this food.");
            }

            var entity = _mapper.Map<IngredientEntity>(createDto);
            _ingredientRepository.Add(entity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating ingredient failed on save.");
            }

            return await Task.FromResult(_mapper.Map<IngredientDto>(entity));
        }

        public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto updateDto)
        {
            var entity = _ingredientRepository.GetSingle(id);
            if (entity == null)
            {
                return null;
            }

            // Partial update: preserve existing values when incoming values are empty/default.
            entity.Name = string.IsNullOrWhiteSpace(updateDto.Name) ? entity.Name : updateDto.Name;
            entity.Quantity = updateDto.Quantity > 0 ? updateDto.Quantity : entity.Quantity;
            entity.FoodEntityId = updateDto.FoodEntityId > 0 ? updateDto.FoodEntityId : entity.FoodEntityId;

            _ingredientRepository.Update(id, entity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating ingredient failed on save.");
            }

            return await Task.FromResult(_mapper.Map<IngredientDto>(entity));
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
                throw new Exception("Deleting ingredient failed on save.");
            }

            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string searchTerm, QueryParameters queryParameters)
        {
            var parameters = new QueryParameters
            {
                Query = searchTerm ?? string.Empty,
                Page = queryParameters.Page,
                PageCount = queryParameters.PageCount
            };

            var entities = _ingredientRepository.GetAll(parameters);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(entities));
        }
    }
}
