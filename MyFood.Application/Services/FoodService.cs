using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

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

        public async Task<IEnumerable<FoodDto>> GetAllFoodsAsync(QueryParameters queryParameters)
        {
            var foodEntities = _foodRepository.GetAll(queryParameters);
            return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foodEntities));
        }

        public async Task<FoodDto?> GetFoodByIdAsync(int id)
        {
            var foodEntity = _foodRepository.GetSingle(id);
            if (foodEntity == null)
            {
                return null;
            }

            return await Task.FromResult(_mapper.Map<FoodDto>(foodEntity));
        }

        public async Task<IEnumerable<FoodDto>> SearchFoodsByNameAsync(string name)
        {
            var foodEntities = _foodRepository.SearchFoodsByName(name);
            return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foodEntities));
        }

        public async Task<FoodDto> CreateFoodAsync(FoodCreateDto foodCreateDto)
        {
            if (foodCreateDto == null)
            {
                throw new ArgumentNullException(nameof(foodCreateDto));
            }

            var foodEntity = _mapper.Map<FoodEntity>(foodCreateDto);
            _foodRepository.Add(foodEntity);

            if (!_foodRepository.Save())
            {
                throw new InvalidOperationException("Creating a food item failed on save.");
            }

            var createdEntity = _foodRepository.GetSingle(foodEntity.Id) ?? foodEntity;
            return await Task.FromResult(_mapper.Map<FoodDto>(createdEntity));
        }

        public async Task<FoodDto?> UpdateFoodAsync(int id, FoodUpdateDto foodUpdateDto)
        {
            if (foodUpdateDto == null)
            {
                throw new ArgumentNullException(nameof(foodUpdateDto));
            }

            var existingEntity = _foodRepository.GetSingle(id);

            if (existingEntity == null)
            {
                return null;
            }

            _mapper.Map(foodUpdateDto, existingEntity);
            var updatedEntity = _foodRepository.Update(id, existingEntity);

            if (!_foodRepository.Save())
            {
                throw new InvalidOperationException("Updating a food item failed on save.");
            }

            return await Task.FromResult(_mapper.Map<FoodDto>(updatedEntity));
        }

        public async Task<bool> DeleteFoodAsync(int id)
        {
            var existingEntity = _foodRepository.GetSingle(id);

            if (existingEntity == null)
            {
                return false;
            }

            _foodRepository.Delete(id);

            if (!_foodRepository.Save())
            {
                throw new InvalidOperationException("Deleting a food item failed on save.");
            }

            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<FoodDto>> GetRandomMealAsync()
        {
            var foodEntities = _foodRepository.GetRandomMeal();
            return await Task.FromResult(_mapper.Map<IEnumerable<FoodDto>>(foodEntities));
        }

        public async Task<int> GetTotalFoodCountAsync()
        {
            var count = _foodRepository.Count();
            return await Task.FromResult(count);
        }
    }
}
