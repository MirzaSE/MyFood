using AutoMapper;
using MyFood.Application;
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

        public async Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync(QueryParameters? queryParameters = null)
        {
            var ingredients = _ingredientRepository.GetAll();

            if (queryParameters != null)
            {
                ingredients = ingredients
                    .Skip((queryParameters.Page - 1) * queryParameters.PageCount)
                    .Take(queryParameters.PageCount);
            }

            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(ingredients.ToList()));
        }

        public async Task<IngredientDto?> GetIngredientByIdAsync(int id)
        {
            if (id < 0)
            {
                return null;
            }

            var ingredient = _ingredientRepository.GetSingle(id);
            return await Task.FromResult(ingredient != null ? _mapper.Map<IngredientDto>(ingredient) : null);
        }

        public async Task<IEnumerable<IngredientDto>> SearchIngredientsByNameAsync(string name)
        {
            var ingredients = _ingredientRepository.SearchIngredientsByName(name);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(ingredients));
        }

        public async Task<IngredientDto> CreateIngredientAsync(CreateIngredientDto createIngredientDto)
        {
            if (string.IsNullOrWhiteSpace(createIngredientDto?.Name))
            {
                throw new ArgumentException("Ingredient name is required.", nameof(createIngredientDto));
            }

            var normalizedName = createIngredientDto.Name.Trim();
            var duplicateExists = _ingredientRepository
                .GetAll()
                .Any(x => x.Name != null && x.Name.ToLowerInvariant() == normalizedName.ToLowerInvariant());

            if (duplicateExists)
            {
                throw new InvalidOperationException("An ingredient with the same name already exists.");
            }

            createIngredientDto.Name = normalizedName;
            var ingredient = _mapper.Map<IngredientEntity>(createIngredientDto);
            _ingredientRepository.Add(ingredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            var createdIngredient = _ingredientRepository.GetSingle(ingredient.Id)
                ?? throw new Exception("Created ingredient could not be retrieved.");

            return await Task.FromResult(_mapper.Map<IngredientDto>(createdIngredient));
        }

        public async Task<IngredientDto?> UpdateIngredientAsync(int id, UpdateIngredientDto updateIngredientDto)
        {
            var existingIngredient = _ingredientRepository.GetSingle(id);
            if (existingIngredient == null)
            {
                return null;
            }

            if (updateIngredientDto?.Name != null && string.IsNullOrWhiteSpace(updateIngredientDto.Name))
            {
                throw new ArgumentException("Ingredient name is required.", nameof(updateIngredientDto));
            }

            _mapper.Map(updateIngredientDto, existingIngredient);
            var updatedIngredient = _ingredientRepository.Update(id, existingIngredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return await Task.FromResult(_mapper.Map<IngredientDto>(updatedIngredient));
        }

        public async Task<bool> DeleteIngredientAsync(int id)
        {
            var ingredient = _ingredientRepository.GetSingle(id);
            if (ingredient == null)
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
    }
}