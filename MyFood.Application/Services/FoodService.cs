using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;
using MyFood.Application.Services;  // IFoodRepository is here
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFood.Application.Services
{
    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IMapper _mapper;

        public FoodService(IFoodRepository foodRepository, IMapper mapper)
        {
            _foodRepository = foodRepository;
            _mapper = mapper;
        }

        public Task<IEnumerable<FoodDto>> GetAllFoodsAsync(QueryParameters queryParameters)
        {
            var foodEntities = _foodRepository.GetAll(queryParameters);
            var dtos = _mapper.Map<IEnumerable<FoodDto>>(foodEntities);
            return Task.FromResult(dtos);
        }

        public Task<FoodDto?> GetFoodByIdAsync(int id)
        {
            var foodEntity = _foodRepository.GetSingle(id);
            var dto = _mapper.Map<FoodDto>(foodEntity);
            return Task.FromResult(dto);
        }

        public Task<(IEnumerable<FoodDto> foodDtos, int totalCount)> SearchFoodsByNameAsync(string name, QueryParameters queryParameters)
        {
            var foodEntities = _foodRepository.SearchFoodsByName(name);
            var totalCount = foodEntities.Count();
            
            var pagedEntities = foodEntities
                .Skip((queryParameters.Page - 1) * queryParameters.PageCount)
                .Take(queryParameters.PageCount);
            
            var dtos = _mapper.Map<IEnumerable<FoodDto>>(pagedEntities);
            return Task.FromResult((dtos, totalCount));
        }

        public async Task<FoodDto> AddFoodAsync(FoodCreateDto foodCreateDto)
        {
            var toAdd = _mapper.Map<FoodEntity>(foodCreateDto);
            _foodRepository.Add(toAdd);
            
            if (!_foodRepository.Save())
                throw new Exception("Creating a fooditem failed on save.");
            
            var newFoodItem = _foodRepository.GetSingle(toAdd.Id);
            return _mapper.Map<FoodDto>(newFoodItem);
        }

        public async Task<FoodDto?> UpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto)
        {
            var existingFoodItem = _foodRepository.GetSingle(id);
            if (existingFoodItem == null) return null;
            
            _mapper.Map(foodUpdateDto, existingFoodItem);
            _foodRepository.Update(id, existingFoodItem);
            
            if (!_foodRepository.Save())
                throw new Exception("Updating a fooditem failed on save.");
            
            return _mapper.Map<FoodDto>(existingFoodItem);
        }

        public async Task<FoodDto?> PartiallyUpdateFoodAsync(int id, JsonPatchDocument<FoodUpdateDto> patchDoc)
        {
            var existingEntity = _foodRepository.GetSingle(id);
            if (existingEntity == null) return null;
            
            var foodUpdateDto = _mapper.Map<FoodUpdateDto>(existingEntity);
            patchDoc.ApplyTo(foodUpdateDto);
            
            _mapper.Map(foodUpdateDto, existingEntity);
            var updated = _foodRepository.Update(id, existingEntity);
            
            if (!_foodRepository.Save())
                throw new Exception("Updating a fooditem failed on save.");
            
            return _mapper.Map<FoodDto>(updated);
        }

        public async Task<bool> DeleteFoodAsync(int id)
        {
            var foodItem = _foodRepository.GetSingle(id);
            if (foodItem == null) return false;
            
            _foodRepository.Delete(id);
            if (!_foodRepository.Save())
                throw new Exception("Deleting a fooditem failed on save.");
            
            return true;
        }

        public Task<IEnumerable<FoodDto>> GetRandomMealAsync()
        {
            var foodEntities = _foodRepository.GetRandomMeal();
            var dtos = _mapper.Map<IEnumerable<FoodDto>>(foodEntities);
            return Task.FromResult(dtos);
        }

        public Task<int> GetTotalFoodCountAsync()
        {
            return Task.FromResult(_foodRepository.Count());
        }
    }
}
