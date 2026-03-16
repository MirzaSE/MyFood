using AutoMapper;
using MyFood.Application.Entities;
using MyFood.Application.Dtos;

namespace MyFood.Api.MappingProfiles
{
    public class IngredientMappings : Profile
    {
        public IngredientMappings()
        {
            CreateMap<IngredientEntity, IngredientDto>();
            CreateMap<IngredientCreateDto, IngredientEntity>();
            CreateMap<IngredientUpdateDto, IngredientEntity>();
        }
    }
}