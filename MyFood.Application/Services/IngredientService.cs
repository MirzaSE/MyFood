using AutoMapper;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services;

public class IngredientService : IIngredientService
{
    private const string SaveCreateError = "Creating an ingredient failed on save.";
    private const string SaveUpdateError = "Updating ingredient failed on save.";
    private const string SaveDeleteError = "Deleting ingredient failed on save.";

    private readonly IIngredientRepository _ingredientRepository;
    private readonly IMapper _mapper;

    public IngredientService(IIngredientRepository ingredientRepository, IMapper mapper)
    {
        _ingredientRepository = ingredientRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<IngredientDto>> GetAllAsync(QueryParameters queryParameters)
    {
        var page = Math.Max(queryParameters.Page, 1);
        var pageCount = Math.Max(queryParameters.PageCount, 1);

        var ingredientEntities = _ingredientRepository
            .GetAll()
            .Skip((page - 1) * pageCount)
            .Take(pageCount);

        return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(ingredientEntities));
    }

    public async Task<IngredientDto?> GetByIdAsync(int id)
    {
        if (id < 1)
        {
            return await Task.FromResult<IngredientDto?>(null);
        }

        var ingredientEntity = _ingredientRepository.GetById(id);
        return await Task.FromResult(ingredientEntity != null ? _mapper.Map<IngredientDto>(ingredientEntity) : null);
    }

    public async Task<IngredientDto> CreateAsync(IngredientCreateDto ingredientCreateDto)
    {
        ArgumentNullException.ThrowIfNull(ingredientCreateDto);

        var name = NormalizeRequiredText(ingredientCreateDto.Name, "Ingredient name is required.");
        var unit = NormalizeRequiredText(ingredientCreateDto.Unit, "Ingredient unit is required.");

        EnsureNameIsUnique(name);

        var ingredientEntity = _mapper.Map<IngredientEntity>(ingredientCreateDto);
        ingredientEntity.Name = name;
        ingredientEntity.Unit = unit;
        ingredientEntity.Created = DateTime.UtcNow;

        _ingredientRepository.Add(ingredientEntity);

        if (!_ingredientRepository.Save())
        {
            throw new Exception(SaveCreateError);
        }

        return await Task.FromResult(_mapper.Map<IngredientDto>(ingredientEntity));
    }

    public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto ingredientUpdateDto)
    {
        ArgumentNullException.ThrowIfNull(ingredientUpdateDto);

        if (id < 1)
        {
            return await Task.FromResult<IngredientDto?>(null);
        }

        var existingEntity = _ingredientRepository.GetById(id);
        if (existingEntity == null)
        {
            return await Task.FromResult<IngredientDto?>(null);
        }

        ApplyPartialUpdate(existingEntity, ingredientUpdateDto);

        var updatedEntity = _ingredientRepository.Update(id, existingEntity);

        if (!_ingredientRepository.Save())
        {
            throw new Exception(SaveUpdateError);
        }

        return await Task.FromResult(_mapper.Map<IngredientDto>(updatedEntity));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (id < 1)
        {
            return await Task.FromResult(false);
        }

        var ingredientEntity = _ingredientRepository.GetById(id);
        if (ingredientEntity == null)
        {
            return await Task.FromResult(false);
        }

        _ingredientRepository.Delete(id);

        if (!_ingredientRepository.Save())
        {
            throw new Exception(SaveDeleteError);
        }

        return await Task.FromResult(true);
    }

    public async Task<IEnumerable<IngredientDto>> SearchAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await Task.FromResult(Enumerable.Empty<IngredientDto>());
        }

        var normalizedName = name.Trim();
        var ingredientEntities = _ingredientRepository
            .GetAll()
            .Where(ingredient => ingredient.Name.Contains(normalizedName, StringComparison.OrdinalIgnoreCase));

        return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(ingredientEntities));
    }

    private void ApplyPartialUpdate(IngredientEntity existingEntity, IngredientUpdateDto ingredientUpdateDto)
    {
        if (!string.IsNullOrWhiteSpace(ingredientUpdateDto.Name))
        {
            var name = ingredientUpdateDto.Name.Trim();
            EnsureNameIsUnique(name, existingEntity.Id);
            existingEntity.Name = name;
        }

        if (!string.IsNullOrWhiteSpace(ingredientUpdateDto.Unit))
        {
            existingEntity.Unit = ingredientUpdateDto.Unit.Trim();
        }

        if (ingredientUpdateDto.Quantity.HasValue)
        {
            existingEntity.Quantity = ingredientUpdateDto.Quantity.Value;
        }

        if (ingredientUpdateDto.CaloriesPerUnit.HasValue)
        {
            existingEntity.CaloriesPerUnit = ingredientUpdateDto.CaloriesPerUnit.Value;
        }

        if (ingredientUpdateDto.Protein.HasValue)
        {
            existingEntity.Protein = ingredientUpdateDto.Protein.Value;
        }

        if (ingredientUpdateDto.Carbs.HasValue)
        {
            existingEntity.Carbs = ingredientUpdateDto.Carbs.Value;
        }

        if (ingredientUpdateDto.Fat.HasValue)
        {
            existingEntity.Fat = ingredientUpdateDto.Fat.Value;
        }
    }

    private void EnsureNameIsUnique(string name, int? currentIngredientId = null)
    {
        var hasDuplicate = _ingredientRepository
            .GetAll()
            .Any(ingredient =>
                ingredient.Id != currentIngredientId &&
                string.Equals(ingredient.Name, name, StringComparison.OrdinalIgnoreCase));

        if (hasDuplicate)
        {
            throw new InvalidOperationException("An ingredient with this name already exists.");
        }
    }

    private static string NormalizeRequiredText(string value, string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(errorMessage);
        }

        return value.Trim();
    }
}
