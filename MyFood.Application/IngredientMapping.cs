using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;

namespace MyFood.Application;

public class IngredientMapping : Profile
{
    public IngredientMapping()
    {
        CreateMap<IngredientEntity, IngredientDto>().ReverseMap();
        CreateMap<IngredientEntity, IngredientUpdateDto>().ReverseMap();
        CreateMap<IngredientEntity, IngredientCreateDto>().ReverseMap();
    }
}