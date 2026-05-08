using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;


namespace MyFood.Application.Services;

public class IngredientService
{
    private readonly IIngredientRepository _repository;
    private readonly IMapper _mapper;

    public IngredientService(
        IIngredientRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync(QueryParameters queryParameters)
    {
        var ingredients = _repository.GetAll(queryParameters).ToList();

        return _mapper.Map<IEnumerable<IngredientDto>>(ingredients);
    }

    public async Task<IngredientDto?> GetIngredientByIdAsync(int id)
    {
        var ingredient = _repository.GetSingle(id);

        if (ingredient == null)
            return null;

        return _mapper.Map<IngredientDto>(ingredient);
    }

    public async Task<IngredientDto> CreateIngredientAsync(IngredientCreateDto createDto)
    {
        var ingredient = _mapper.Map<IngredientEntity>(createDto);

        _repository.Add(ingredient);

        _repository.Save();

        return _mapper.Map<IngredientDto>(ingredient);
    }

    public async Task<IngredientDto?> UpdateIngredientAsync(int id, IngredientUpdateDto updateDto)
    {
        var existing = _repository.GetSingle(id);

        if (existing == null)
            return null;

        _mapper.Map(updateDto, existing);

        var updated = _repository.Update(id, existing);

        _repository.Save();

        return _mapper.Map<IngredientDto>(updated);
    }

    public async Task<bool> DeleteIngredientAsync(int id)
    {
        var existing = _repository.GetSingle(id);

        if (existing == null)
            return false;

        _repository.Delete(id);

        return _repository.Save();
    }
}