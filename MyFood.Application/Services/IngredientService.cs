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
            var ingredients = ApplyPaging(_ingredientRepository.GetAll(), queryParameters);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(ingredients.ToList()));
        }

        public async Task<int> GetTotalCountAsync(string? searchTerm = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await Task.FromResult(_ingredientRepository.Count());
            }

            return await Task.FromResult(_ingredientRepository.SearchByName(searchTerm.Trim()).Count());
        }

        public async Task<IngredientDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var entity = _ingredientRepository.GetSingle(id);
            return await Task.FromResult(entity == null ? null : _mapper.Map<IngredientDto>(entity));
        }

        public async Task<IngredientDto> CreateAsync(IngredientCreateDto ingredientCreateDto)
        {
            ValidateCreateOrUpdate(ingredientCreateDto.Name);

            if (_ingredientRepository.GetByExactName(ingredientCreateDto.Name.Trim()) != null)
            {
                throw new InvalidOperationException("An ingredient with the same name already exists.");
            }

            var entity = _mapper.Map<IngredientEntity>(ingredientCreateDto);
            entity.Name = ingredientCreateDto.Name.Trim();
            entity.Unit = ingredientCreateDto.Unit.Trim();

            _ingredientRepository.Add(entity);

            if (!_ingredientRepository.Save())
            {
                throw new Exception("Creating an ingredient failed on save.");
            }

            return await Task.FromResult(_mapper.Map<IngredientDto>(entity));
        }

        public async Task<IngredientDto?> UpdateAsync(int id, IngredientUpdateDto ingredientUpdateDto)
        {
            if (id <= 0)
            {
                return null;
            }

            ValidateCreateOrUpdate(ingredientUpdateDto.Name);

            var existing = _ingredientRepository.GetSingle(id);
            if (existing == null)
            {
                return null;
            }

            var duplicate = _ingredientRepository.GetByExactName(ingredientUpdateDto.Name.Trim());
            if (duplicate != null && duplicate.Id != id)
            {
                throw new InvalidOperationException("An ingredient with the same name already exists.");
            }

            _mapper.Map(ingredientUpdateDto, existing);
            existing.Name = ingredientUpdateDto.Name.Trim();
            existing.Unit = ingredientUpdateDto.Unit.Trim();

            var updated = _ingredientRepository.Update(id, existing);
            if (!_ingredientRepository.Save())
            {
                throw new Exception("Updating an ingredient failed on save.");
            }

            return await Task.FromResult(_mapper.Map<IngredientDto>(updated));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            var existing = _ingredientRepository.GetSingle(id);
            if (existing == null)
            {
                return false;
            }

            _ingredientRepository.Delete(id);
            if (!_ingredientRepository.Save())
            {
                throw new Exception("Deleting an ingredient failed on save.");
            }

            return true;
        }

        public async Task<IEnumerable<IngredientDto>> SearchAsync(string searchTerm, QueryParameters queryParameters)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<IngredientDto>();
            }

            var query = _ingredientRepository.SearchByName(searchTerm.Trim());
            var paged = ApplyPaging(query, queryParameters);
            return await Task.FromResult(_mapper.Map<IEnumerable<IngredientDto>>(paged.ToList()));
        }

        private static IQueryable<IngredientEntity> ApplyPaging(IQueryable<IngredientEntity> query, QueryParameters queryParameters)
        {
            return query
                .OrderBy(x => x.Name)
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        private static void ValidateCreateOrUpdate(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Ingredient name is required.");
            }
        }
    }
}
