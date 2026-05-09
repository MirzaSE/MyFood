using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;

namespace MyFood.Application.Services;

public class IngredientService : IIngredientService
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IMapper _mapper;

    public IngredientService(IIngredientRepository ingredientRepository, IMapper mapper)
    {
        _ingredientRepository = ingredientRepository;
        _mapper = mapper;
    }

    public Task<IngredientDto> CreateIngredientAsync(IngredientCreateDto ingredientCreateDto)
    {
        var ingredientEntity = _mapper.Map<IngredientEntity>(ingredientCreateDto);

        _ingredientRepository.Add(ingredientEntity);
        _ingredientRepository.Save();

        return Task.FromResult(_mapper.Map<IngredientDto>(ingredientEntity));
    }

    public Task<bool> DeleteIngredientAsync(int id)
    {
        var ingredient = _ingredientRepository.GetSingle(id);

        if (ingredient == null)
        {
            return Task.FromResult(false);
        }

        _ingredientRepository.Delete(id);

        return Task.FromResult(_ingredientRepository.Save());
    }

    public Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync(QueryParameters queryParameters)
    {
        var ingredientEntities = _ingredientRepository.GetAll(queryParameters);

        return Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(ingredientEntities));
    }

    public Task<IngredientDto?> GetIngredientByIdAsync(int id)
    {
        var ingredientEntity = _ingredientRepository.GetSingle(id);

        return Task.FromResult(ingredientEntity != null ? _mapper.Map<IngredientDto>(ingredientEntity) : null);
    }

    public Task<int> GetTotalIngredientCountAsync()
    {
        return Task.FromResult(_ingredientRepository.Count());
    }

    public Task<IEnumerable<IngredientDto>> SearchIngredientsByNameAsync(string name)
    {
        var ingredientEntities = _ingredientRepository.SearchIngredientsByName(name);

        return Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(ingredientEntities));
    }

    public Task<IngredientDto?> UpdateIngredientAsync(int id, IngredientUpdateDto ingredientUpdateDto)
    {
        var existingIngredientEntity = _ingredientRepository.GetSingle(id);

        if (existingIngredientEntity == null)
        {
            return Task.FromResult<IngredientDto?>(null);
        }

        _mapper.Map(ingredientUpdateDto, existingIngredientEntity);
        _ingredientRepository.Update(id, existingIngredientEntity);
        _ingredientRepository.Save();

        return Task.FromResult(_mapper.Map<IngredientDto?>(existingIngredientEntity));
    }
}