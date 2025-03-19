using MyFood.Application;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
    {
        private readonly FoodDbContext _foodDbContext;

        public IngredientSqlRepository(FoodDbContext foodDbContext)
        {
            _foodDbContext = foodDbContext;
        }

        public IngredientEntity GetSingle(int id)
        {
            return _foodDbContext.Ingredients.FirstOrDefault(x => x.Id == id);
        }

        public void Add(IngredientEntity item)
        {
            _foodDbContext.Ingredients.Add(item);
            _foodDbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var ingredient = _foodDbContext.Ingredients.FirstOrDefault(x => x.Id == id);
            if (ingredient != null)
            {
                _foodDbContext.Ingredients.Remove(ingredient);
                _foodDbContext.SaveChanges();
            }
        }

        public IngredientEntity Update(int id, IngredientEntity item)
        {
            var existingIngredient = _foodDbContext.Ingredients.FirstOrDefault(x => x.Id == id);
            if (existingIngredient != null)
            {
                existingIngredient.Name = item.Name;
                existingIngredient.Quantity = item.Quantity;
                _foodDbContext.SaveChanges();
            }
            return existingIngredient;
        }

        public IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<IngredientEntity> _allItems = _foodDbContext.Ingredients.OrderBy(x => x.Name);

            if (queryParameters.HasQuery())
            {
                _allItems = _allItems
                    .Where(x => x.Name.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant()));
            }

            return _allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public int Count()
        {
            return _foodDbContext.Ingredients.Count();
        }

        public bool Save()
        {
            return (_foodDbContext.SaveChanges() >= 0);
        }

    }
}