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
            var ingredientEntities = _ingredientRepository.GetAll(queryParameters);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(ingredientEntities));
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Ingredient id cannot be negative.");
            }

            var ingredientEntity = _ingredientRepository.GetSingle(id);
            return await Task.FromResult(ingredientEntity != null ? _mapper.Map<IngredientDto>(ingredientEntity) : null);
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto ingredientCreateDto)
        {
            if (string.IsNullOrWhiteSpace(ingredientCreateDto.Name))
            {
                throw new ArgumentException("Ingredient name is required.", nameof(ingredientCreateDto));
            }
            var normalizedName = ingredientCreateDto.Name.Trim().ToLower();

            var duplicates = _ingredientRepository.GetAll(new QueryParameters
            {
                Page = 1,
                PageCount = 50
            });

            if (duplicates.Any(x =>
                    x.Name != null &&
                    x.Name.ToLower() == normalizedName &&
                    x.FoodEntityId == ingredientCreateDto.FoodEntityId))
            {
                throw new InvalidOperationException("Ingredient with the same name already exists for this food.");
            }

            var ingredientEntity = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            _ingredientRepository.Add(ingredientEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            var newIngredientEntity = _ingredientRepository.GetSingle(ingredientEntity.Id);
            return await Task.FromResult(_mapper.Map<IngredientDto>(newIngredientEntity));
        }

        public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto ingredientUpdateDto)
        {
            var existingEntity = _ingredientRepository.GetSingle(id);
            if (existingEntity == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(ingredientUpdateDto.Name))
            {
                existingEntity.Name = ingredientUpdateDto.Name;
            }

            if (ingredientUpdateDto.Quantity > 0)
            {
                existingEntity.Quantity = ingredientUpdateDto.Quantity;
            }

            if (ingredientUpdateDto.Protein > 0)
            {
                existingEntity.Protein = ingredientUpdateDto.Protein;
            }

            if (ingredientUpdateDto.Carbs > 0)
            {
                existingEntity.Carbs = ingredientUpdateDto.Carbs;
            }

            if (ingredientUpdateDto.Fat > 0)
            {
                existingEntity.Fat = ingredientUpdateDto.Fat;
            }

            if (ingredientUpdateDto.FoodEntityId > 0)
            {
                existingEntity.FoodEntityId = ingredientUpdateDto.FoodEntityId;
            }

            var updatedEntity = _ingredientRepository.Update(id, existingEntity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return await Task.FromResult(_mapper.Map<IngredientDto>(updatedEntity));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ingredientEntity = _ingredientRepository.GetSingle(id);
            if (ingredientEntity == null)
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

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string query)
        {
            var queryParameters = new QueryParameters
            {
                Page = 1,
                PageCount = 50,
                Query = query
            };

            var ingredientEntities = _ingredientRepository.GetAll(queryParameters);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(ingredientEntities));
        }

        public async Task<int> GetTotalIngredientCountAsync()
        {
            return await Task.FromResult(_ingredientRepository.Count());
        }
    }
}
