using Microsoft.EntityFrameworkCore;
using MyFood.Application;
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
}