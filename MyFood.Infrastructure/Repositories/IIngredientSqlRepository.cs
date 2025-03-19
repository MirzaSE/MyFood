using MyFood.Application;
using MyFood.Application.Entities;
using MyFood.Infrastructure.Helpers;

namespace MyFood.Infrastructure.Repositories;

public interface IIngredientSqlRepository
{
    Task<ServiceResponse<IngredientEntity>> GetSingle(int id);
    Task<ServiceResponse<IngredientEntity>> Add(IngredientEntity item);
    Task<ServiceResponse<List<IngredientEntity>>> GetAll();

}