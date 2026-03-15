using MyFood.Application;
using MyFood.Application.Entities;
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
        return _foodDbContext.IngredientItems.FirstOrDefault(x => x.Id == id);
    }

    public void Add(IngredientEntity item)
    {
        _foodDbContext.IngredientItems.Add(item);
    }

    public void Delete(int id)
    {
        IngredientEntity? ingredientItem = GetSingle(id);
        if (ingredientItem != null)
        {
            _foodDbContext.IngredientItems.Remove(ingredientItem);
        }
    }

    public IngredientEntity Update(int id, IngredientEntity item)
    {
        _foodDbContext.IngredientItems.Update(item);
        return item;
    }

    public IQueryable<IngredientEntity> GetAll(QueryParameters queryParameters)
    {
        IQueryable<IngredientEntity> allItems = _foodDbContext.IngredientItems.OrderBy(x => x.Name);

        if (queryParameters.HasQuery())
        {
            string filter = queryParameters.Query!.ToLowerInvariant();
            allItems = allItems.Where(x =>
                x.Quantity.ToString().Contains(filter) ||
                (x.Name != null && x.Name.ToLowerInvariant().Contains(filter)));
        }

        return allItems
            .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
            .Take(queryParameters.PageCount);
    }

    public int Count()
    {
        return _foodDbContext.IngredientItems.Count();
    }

    public bool Save()
    {
        return _foodDbContext.SaveChanges() >= 0;
    }
}
