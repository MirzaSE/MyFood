using MyFood.Application.Entities;  // For IngredientEntity
using MyFood.Infrastructure.Repositories;  // For FoodDbContext
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
public class IngredientSqlRepository : IIngredientRepository
{
    private readonly FoodDbContext _context;

    public IngredientSqlRepository(FoodDbContext context)
    {
        _context = context;
    }

    public void AddIngredient(IngredientEntity ingredient)
    {
        _context.Ingredients.Add(ingredient);
        _context.SaveChanges();
    }

    public IEnumerable<IngredientEntity> GetAllIngredients()
    {
        return _context.Ingredients.ToList();
    }

    public IngredientEntity GetIngredientById(int id)
    {
        return _context.Ingredients.FirstOrDefault(i => i.Id == id);
    }

    public void UpdateIngredient(IngredientEntity ingredient)
    {
        _context.Ingredients.Update(ingredient);
        _context.SaveChanges();
    }

    public void DeleteIngredient(int id)
    {
        var ingredient = _context.Ingredients.Find(id);
        if (ingredient != null)
        {
            _context.Ingredients.Remove(ingredient);
            _context.SaveChanges();
        }
    }

}
