using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application.Services;

public class IngredientService : IIngredientService
{
    private readonly IIngredientRepository _repository;
    private readonly IMapper _mapper;

    public IngredientService(IIngredientRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<IngredientDto>> GetAllAsync(QueryParameters queryParameters)
    {
        var entities = _repository.GetAll(queryParameters);
        return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(entities));
    }

    public async Task<IngredientDto?> GetByIdAsync(int id)
    {
        var entity = _repository.GetSingle(id);
        return await Task.FromResult(entity != null ? _mapper.Map<IngredientDto>(entity) : null);
    }

    public async Task<IngredientDto> CreateAsync(IngredientCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name is required.");

        var existing = _repository.SearchByName(dto.Name)
            .FirstOrDefault(x => x.Name?.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) == true);
        if (existing != null)
            throw new InvalidOperationException($"An ingredient with name '{dto.Name}' already exists.");

        var entity = _mapper.Map<IngredientEntity>(dto);
        _repository.Add(entity);

        if (!_repository.Save())
            throw new Exception("Creating an ingredient failed on save.");

        var created = _repository.GetSingle(entity.Id);
        return await Task.FromResult(_mapper.Map<IngredientDto>(created!));
    }

    public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto dto)
    {
        var entity = _repository.GetSingle(id);
        if (entity == null)
            return null;

        _mapper.Map(dto, entity);
        _repository.Update(id, entity);

        if (!_repository.Save())
            throw new Exception("Updating an ingredient failed on save.");

        return await Task.FromResult(_mapper.Map<IngredientDto>(entity));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = _repository.GetSingle(id);
        if (entity == null)
            return false;

        _repository.Delete(id);

        if (!_repository.Save())
            throw new Exception("Deleting an ingredient failed on save.");

        return await Task.FromResult(true);
    }

    public async Task<IEnumerable<IngredientDto>> SearchAsync(string name)
    {
        var entities = _repository.SearchByName(name);
        return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(entities));
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await Task.FromResult(_repository.Count());
    }
}
