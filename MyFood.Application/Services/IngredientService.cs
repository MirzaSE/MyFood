using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFood.Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _repository;
        private readonly IMapper _mapper;

        public IngredientService(IIngredientRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<IngredientDto> Data, int TotalCount)> GetAllAsync(QueryParameters queryParameters)
        {
            var query = _repository.GetAll();
            var totalCount = query.Count();
            
            var pagedIngredients = query
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount)
                .ToList();

            var dtos = _mapper.Map<IEnumerable<IngredientDto>>(pagedIngredients);
            return await Task.FromResult((dtos, totalCount));
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id <= 0) return null;
            
            var ingredient = _repository.GetSingle(id);
            return await Task.FromResult(ingredient == null ? null : _mapper.Map<IngredientDto>(ingredient));
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.Name))
                throw new ArgumentException("Name cannot be null or empty.");

            var allIngredients = _repository.GetAll().ToList();
            if (allIngredients.Any(i => i.Name.Equals(createDto.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Ingredient with this name already exists.");

            var entity = _mapper.Map<IngredientEntity>(createDto);
            _repository.Add(entity);
            _repository.Save(); // Your repository requires Save() to be called
            
            return await Task.FromResult(_mapper.Map<IngredientDto>(entity));
        }

        public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto updateDto)
        {
            var existing = _repository.GetSingle(id);
            if (existing == null) return null;

            _mapper.Map(updateDto, existing);
            var updated = _repository.Update(existing);
            _repository.Save();
            
            return await Task.FromResult(_mapper.Map<IngredientDto>(updated));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = _repository.GetSingle(id);
            if (existing == null) return false;

            _repository.Delete(existing);
            _repository.Save();
            
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string searchTerm)
        {
            var query = _repository.GetAll();
            if (string.IsNullOrWhiteSpace(searchTerm)) return Enumerable.Empty<IngredientDto>();

            var filtered = query.ToList() // Execute query before string manipulation
                .Where(i => i.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(filtered));
        }
    }
}