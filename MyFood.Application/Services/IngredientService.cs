using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using MyFood.Application.Repositories;  

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

        public async Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync(QueryParameters queryParameters)
        {
            var ingredients = await _ingredientRepository.GetAllAsync();
            
            // Apply pagination
            var pagedIngredients = ingredients
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
            
            return _mapper.Map<IEnumerable<IngredientDto>>(pagedIngredients);
        }

        public async Task<IngredientDto?> GetIngredientByIdAsync(int id)
        {
            if (id < 0) return null;
            
            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            return ingredient != null ? _mapper.Map<IngredientDto>(ingredient) : null;
        }

        public async Task<IEnumerable<IngredientDto>> SearchIngredientsByNameAsync(string name)
        {
            var ingredients = await _ingredientRepository.GetAllAsync();
            var filtered = ingredients.Where(x => 
                string.IsNullOrEmpty(name) || 
                x.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            
            return _mapper.Map<IEnumerable<IngredientDto>>(filtered);
        }

        public async Task<IngredientDto> CreateIngredientAsync(IngredientCreateDto ingredientCreateDto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(ingredientCreateDto.Name))
                throw new Exception("Ingredient name is required");
            
            var ingredient = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            var created = await _ingredientRepository.AddAsync(ingredient);
            return _mapper.Map<IngredientDto>(created);
        }

        public async Task<IngredientDto?> UpdateIngredientAsync(int id, IngredientUpdateDto ingredientUpdateDto)
        {
            var existing = await _ingredientRepository.GetByIdAsync(id);
            if (existing == null) return null;
            
            _mapper.Map(ingredientUpdateDto, existing);
            var updated = await _ingredientRepository.UpdateAsync(id, existing);
            return updated != null ? _mapper.Map<IngredientDto>(updated) : null;
        }

        public async Task<bool> DeleteIngredientAsync(int id)
        {
            return await _ingredientRepository.DeleteAsync(id);
        }

        public async Task<int> GetTotalIngredientCountAsync()
        {
            var ingredients = await _ingredientRepository.GetAllAsync();
            return ingredients.Count();
        }
    }
}