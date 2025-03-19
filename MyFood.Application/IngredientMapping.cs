using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;

namespace MyFood.Application
{
    public class IngredientMapping : Profile
    {
        public IngredientMapping()
        {
            CreateMap<IngredientEntity, IngredientEntity>().ReverseMap();
            CreateMap<IngredientEntity, IngredientCreateDto>().ReverseMap();
            CreateMap<IngredientEntity, IngredientUpdateDto>().ReverseMap();
        }
    }
}