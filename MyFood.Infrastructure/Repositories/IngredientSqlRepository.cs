using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using MyFood.Infrastructure.Helpers;

namespace MyFood.Infrastructure.Repositories;

public class IngredientSqlRepository : IIngredientRepository
{
    private readonly FoodDbContext _foodDbContext;

    public IngredientSqlRepository(FoodDbContext foodDbContext)
    {
        _foodDbContext = foodDbContext;
    }

    public IngredientEntity? GetSingle(int id)
    {
        return _foodDbContext.Ingredients.FirstOrDefault(x => x.Id == id);
    }

    public void Add(IngredientEntity item)
    {
        _foodDbContext.Ingredients.Add(item);
    }

    public void Delete(int id)
    {
        var item = GetSingle(id);
        if (item != null)
            _foodDbContext.Ingredients.Remove(item);
    }

    public IngredientEntity Update(int id, IngredientEntity item)
    {
        _foodDbContext.Ingredients.Update(item);
        return item;
    }

    public IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters)
    {
        IQueryable<IngredientEntity> items = _foodDbContext.Ingredients.OrderBy(x => x.Name);

        if (queryParameters.HasQuery())
        {
            var query = queryParameters.Query!.ToLowerInvariant();
            items = items.Where(x => x.Name != null && x.Name.ToLower().Contains(query));
        }

        return items
            .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
            .Take(queryParameters.PageCount);
    }

    public IEnumerable<IngredientEntity> SearchByName(string name)
    {
        return _foodDbContext.Ingredients
            .Where(x => EF.Functions.Like(x.Name, $"%{name}%"))
            .ToList();
    }

    public int Count()
    {
        return _foodDbContext.Ingredients.Count();
    }

    public bool Save()
    {
        return _foodDbContext.SaveChanges() >= 0;
    }
}
