using Microsoft.EntityFrameworkCore;
using MyFood.Application;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Helpers;

namespace MyFood.Infrastructure.Repositories;

public class IngredientSqlRepository : IIngredientSqlRepository
{
    private readonly FoodDbContext _foodDbContext;

    public IngredientSqlRepository(FoodDbContext foodDbContext)
    {
        _foodDbContext = foodDbContext ?? throw new ArgumentNullException(nameof(foodDbContext));
    }

    public async Task<ServiceResponse<IngredientEntity>> Add(IngredientEntity item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        await _foodDbContext.Ingredients.AddAsync(item);
        await _foodDbContext.SaveChangesAsync();

        return new ServiceResponse<IngredientEntity>
        {
            Data = item,
            Success = true
        };
    }

    public async Task<ServiceResponse<List<IngredientEntity>>> GetAll()
    {
        var ingredients = await _foodDbContext.Ingredients.ToListAsync();

        return new ServiceResponse<List<IngredientEntity>>
        {
            Data = ingredients,
            Success = true
        };
    }

    public async Task<ServiceResponse<IngredientEntity>> GetSingle(int id)
    {
        var ingredient = await _foodDbContext.Ingredients
            .FirstOrDefaultAsync(x => x.Id == id);

        if (ingredient == null)
        {
            return new ServiceResponse<IngredientEntity>
            {
                Success = false,
                Message = "Ingredient not found."
            };
        }

        return new ServiceResponse<IngredientEntity>
        {
            Data = ingredient,
            Success = true
        };
    }

    public async Task<ServiceResponse<IngredientEntity>> Update(IngredientUpdateDto item, int id)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        var ingredient = await _foodDbContext.Ingredients
            .FirstOrDefaultAsync(i => i.Id == id);

        if (ingredient == null)
        {
            return new ServiceResponse<IngredientEntity>
            {
                Success = false,
                Message = "Ingredient not found."
            };
        }

        ingredient.Name = item.Name;
        ingredient.Quantity = item.Quantity;

        await _foodDbContext.SaveChangesAsync();

        return new ServiceResponse<IngredientEntity>
        {
            Data = ingredient,
            Success = true
        };
    }

    public async Task<ServiceResponse<bool>> Delete(int id)
    {
        var ingredient = await _foodDbContext.Ingredients
            .FirstOrDefaultAsync(x => x.Id == id);

        if (ingredient == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Data = false,
                Message = "Ingredient not found."
            };
        }

        _foodDbContext.Ingredients.Remove(ingredient);
        await _foodDbContext.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true,
            Success = true
        };
    }
}