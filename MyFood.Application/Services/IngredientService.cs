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

        public async Task<IEnumerable<IngredientDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var ingredients = _ingredientRepository
                .GetAll()
                .OrderBy(i => i.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(ingredients));
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var ingredient = _ingredientRepository.GetById(id);
            return await Task.FromResult(
                ingredient == null ? null : _mapper.Map<IngredientDto>(ingredient)
            );
        }

        public async Task<IngredientDto> CreateAsync(CreateIngredientDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentException("Ingredient data is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Ingredient name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Unit))
            {
                throw new ArgumentException("Ingredient unit is required.");
            }

            if (dto.CaloriesPerUnit <= 0)
            {
                throw new ArgumentException("Calories per unit must be greater than zero.");
            }

            var duplicate = _ingredientRepository.GetByName(dto.Name.Trim());
            if (duplicate != null)
            {
                throw new InvalidOperationException("Ingredient with this name already exists.");
            }

            var ingredient = _mapper.Map<IngredientEntity>(dto);
            ingredient.Name = dto.Name.Trim();
            ingredient.Unit = dto.Unit.Trim();

            _ingredientRepository.Add(ingredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating ingredient failed on save.");
            }

            return await Task.FromResult(_mapper.Map<IngredientDto>(ingredient));
        }

        public async Task<IngredientDto?> UpdateAsync(int id, UpdateIngredientDto dto)
        {
            if (id <= 0 || dto == null)
            {
                return null;
            }

            var ingredient = _ingredientRepository.GetById(id);
            if (ingredient == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                ingredient.Name = dto.Name.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Unit))
            {
                ingredient.Unit = dto.Unit.Trim();
            }

            if (dto.CaloriesPerUnit.HasValue)
            {
                ingredient.CaloriesPerUnit = dto.CaloriesPerUnit.Value;
            }

            if (dto.Protein.HasValue)
            {
                ingredient.Protein = dto.Protein.Value;
            }

            if (dto.Carbs.HasValue)
            {
                ingredient.Carbs = dto.Carbs.Value;
            }

            if (dto.Fat.HasValue)
            {
                ingredient.Fat = dto.Fat.Value;
            }

            if (dto.Quantity.HasValue)
            {
                ingredient.Quantity = dto.Quantity.Value;
            }

            if (dto.FoodEntityId.HasValue)
            {
                ingredient.FoodEntityId = dto.FoodEntityId.Value;
            }

            _ingredientRepository.Update(ingredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating ingredient failed on save.");
            }

            return await Task.FromResult(_mapper.Map<IngredientDto>(ingredient));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            var ingredient = _ingredientRepository.GetById(id);
            if (ingredient == null)
            {
                return false;
            }

            _ingredientRepository.Delete(ingredient);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Deleting ingredient failed on save.");
            }

            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await Task.FromResult(Enumerable.Empty<IngredientDto>());
            }

            var normalizedTerm = searchTerm.Trim().ToLower();

            var ingredients = _ingredientRepository
                .GetAll()
                .Where(i => i.Name.ToLower().Contains(normalizedTerm))
                .OrderBy(i => i.Name)
                .ToList();

            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(ingredients));
        }
    }
}