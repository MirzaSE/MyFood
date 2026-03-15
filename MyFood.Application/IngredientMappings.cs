using System;
using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application;

public class IngredientMappings : Profile
{
    public IngredientMappings()
        {
            CreateMap<IngredientEntity, IngredientDto>().ReverseMap();
            CreateMap<IngredientEntity, IngredientUpdateDto>().ReverseMap();
            CreateMap<IngredientEntity, IngredientCreateDto>().ReverseMap();
        }
}
