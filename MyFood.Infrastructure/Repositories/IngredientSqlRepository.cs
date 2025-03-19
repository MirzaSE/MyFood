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
        _foodDbContext = foodDbContext;
    }
    
    public async Task<ServiceResponse<IngredientEntity>> Add(IngredientEntity item)
    {
        var serviceResponse = new ServiceResponse<IngredientEntity>();
        await _foodDbContext.Ingredients.AddAsync(item);
        await _foodDbContext.SaveChangesAsync();
        
        serviceResponse.Data = item;
        serviceResponse.Success = true;
        return serviceResponse;
    }
    

    public async Task<ServiceResponse<List<IngredientEntity>>> GetAll()
    {
        var serviceResponse = new ServiceResponse<List<IngredientEntity>>();
        var ingredients = await _foodDbContext.Ingredients.ToListAsync();
        serviceResponse.Data = ingredients;
        serviceResponse.Success = true;
        return serviceResponse;
    }


    public async Task<ServiceResponse<IngredientEntity>> GetSingle(int id)
    {
        var serviceResponse = new ServiceResponse<IngredientEntity>();
        var ingredient = await _foodDbContext.Ingredients.FirstOrDefaultAsync(x => x.Id == id);
        if (ingredient == null)
        {
            serviceResponse.Success = false;
            return serviceResponse;
        }
        serviceResponse.Data = ingredient;
        serviceResponse.Success = true;
        return serviceResponse;
    }
    public async Task<ServiceResponse<IngredientEntity>> Update(IngredientUpdateDto item, int id)
    {
        var serviceResponse = new ServiceResponse<IngredientEntity>();
        var ingredient = await _foodDbContext.Ingredients.FirstOrDefaultAsync(i => i.Id == id);

        if (ingredient == null)
        {
            serviceResponse.Success = false;
            return serviceResponse;
        }
        ingredient.Name = item.Name;
        ingredient.Quantity = item.Quantity;
        await _foodDbContext.SaveChangesAsync();
        
        serviceResponse.Data = ingredient;
        serviceResponse.Success = true;
        return serviceResponse;
    }

    public async Task<ServiceResponse<bool>> Delete(int id)
    {
        var serviceResponse = new ServiceResponse<bool>();
        var ingredient = await _foodDbContext.Ingredients.FirstOrDefaultAsync(x => x.Id == id);
        if (ingredient == null)
        {
            serviceResponse.Success = false;
            serviceResponse.Data = false;
            return serviceResponse;
        }
        _foodDbContext.Ingredients.Remove(ingredient);
        await _foodDbContext.SaveChangesAsync();
        
        serviceResponse.Data = true;
        serviceResponse.Success = true;
        return serviceResponse;
    }
}
