using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFood.Application.Entities;

using Microsoft.EntityFrameworkCore;

namespace MyFood.Infrastructure.Repositories
{
    public class IngredientSqlRepository : IIngredientRepository
{
    private readonly FoodDbContext _context;

    public IngredientSqlRepository(FoodDbContext context)
    {
        _context = context;
    }

    public async Task<List<IngredientEntity>> GetAllAsync()
    {
        return await _context.Ingredients.ToListAsync();
    }

    public async Task<IngredientEntity?> GetByIdAsync(int id)
    {
        return await _context.Ingredients.FindAsync(id);
    }

    public async Task<IngredientEntity> AddAsync(IngredientEntity ingredient)
    {
        _context.Ingredients.Add(ingredient);
        await _context.SaveChangesAsync();
        return ingredient;
    }

    public async Task<IngredientEntity> UpdateAsync(IngredientEntity ingredient)
    {
        _context.Ingredients.Update(ingredient);
        await _context.SaveChangesAsync();
        return ingredient;
    }

    public async Task DeleteAsync(int id)
    {
        var ingredient = await _context.Ingredients.FindAsync(id);

        if (ingredient != null)
        {
            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
        }
    }
}
}