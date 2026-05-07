using MyFood.Application.Interfaces;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _repository;

        public IngredientService(IIngredientRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<IngredientEntity>> GetAllAsync(int? page = null, int? pageSize = null)
        {
            var all = (await _repository.GetAllAsync()).ToList();
            if (page is null || pageSize is null)
            {
                return all;
            }

            var p = page.Value;
            var size = pageSize.Value;
            if (p < 1 || size < 1)
            {
                return Array.Empty<IngredientEntity>();
            }

            return all.Skip((p - 1) * size).Take(size);
        }

        public async Task<IngredientEntity?> GetByIdAsync(int id)
        {
            if (id < 0)
            {
                return null;
            }

            return await _repository.GetByIdAsync(id);
        }

        public async Task<IngredientEntity> CreateAsync(IngredientEntity ingredient)
        {
            ValidateIngredient(ingredient);

            var name = ingredient.Name.Trim();
            if (await _repository.ExistsByNameAsync(name))
            {
                throw new InvalidOperationException($"An ingredient named '{name}' already exists.");
            }

            ingredient.Name = name;
            ingredient.Unit = ingredient.Unit.Trim();
            return await _repository.CreateAsync(ingredient);
        }

        public async Task<IngredientEntity?> UpdateAsync(int id, IngredientEntity incoming)
        {
            if (incoming is null)
            {
                throw new ArgumentNullException(nameof(incoming));
            }

            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
            {
                return null;
            }

            ValidateIngredient(incoming);

            var name = incoming.Name.Trim();
            if (await _repository.ExistsByNameExceptIdAsync(name, id))
            {
                throw new InvalidOperationException($"An ingredient named '{name}' already exists.");
            }

            existing.Name = name;
            existing.Unit = incoming.Unit.Trim();
            existing.CaloriesPerUnit = incoming.CaloriesPerUnit;
            existing.Protein = incoming.Protein;
            existing.Carbs = incoming.Carbs;
            existing.Fat = incoming.Fat;
            existing.FoodEntityId = incoming.FoodEntityId;

            await _repository.UpdateAsync(existing);
            return existing;
        }

        public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);

        public Task<IEnumerable<IngredientEntity>> SearchAsync(string query)
        {
            var normalized = query?.Trim() ?? string.Empty;
            if (normalized.Length == 0)
            {
                return Task.FromResult<IEnumerable<IngredientEntity>>(Array.Empty<IngredientEntity>());
            }

            return _repository.SearchAsync(normalized.ToLowerInvariant());
        }

        private static void ValidateIngredient(IngredientEntity ingredient)
        {
            if (ingredient.Name is null || string.IsNullOrWhiteSpace(ingredient.Name))
            {
                throw new ArgumentException("Name is required.", nameof(ingredient));
            }

            if (ingredient.Unit is null || string.IsNullOrWhiteSpace(ingredient.Unit))
            {
                throw new ArgumentException("Unit is required.", nameof(ingredient));
            }

            if (ingredient.CaloriesPerUnit <= 0
                || ingredient.Protein <= 0
                || ingredient.Carbs <= 0
                || ingredient.Fat <= 0)
            {
                throw new ArgumentException(
                    "Calories per unit, protein, carbs, and fat must be greater than zero.",
                    nameof(ingredient));
            }
        }
    }
}
