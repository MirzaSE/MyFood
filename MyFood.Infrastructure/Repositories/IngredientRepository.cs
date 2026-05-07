using MyFood.Domain.Entities;
using MyFood.Application.Services;

namespace MyFood.Infrastructure.Repositories;

public class IngredientSqlRepository : IIngredientRepository
{
    private readonly FoodDbContext _context;

    public IngredientSqlRepository(FoodDbContext context)
    {
        _context = context;
    }

    public IEnumerable<IngredientEntity> GetAll()
    {
        return _context.Ingredients
            .Where(ingredient => ingredient.FoodEntityId == null)
            .OrderBy(ingredient => ingredient.Name)
            .ToList();
    }

    public IngredientEntity? GetById(int id)
    {
        return _context.Ingredients.Find(id);
    }

    public IngredientEntity Add(IngredientEntity ingredient)
    {
        _context.Ingredients.Add(ingredient);
        return ingredient;
    }

    public IngredientEntity Update(int id, IngredientEntity ingredient)
    {
        _context.Ingredients.Update(ingredient);
        return ingredient;
    }

    public void Delete(int id)
    {
        var entity = GetById(id);
        if (entity != null)
        {
            _context.Ingredients.Remove(entity);
        }
    }

    public bool Save()
    {
        return _context.SaveChanges() >= 0;
    }
}
