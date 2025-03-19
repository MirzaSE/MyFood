using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;

namespace MyFood.Api.MappingProfiles
{
    public class IngredientMappings : Profile
    {
        public IngredientMappings()
        {
            CreateMap<IngredientEntity, IngredientDto>();
            CreateMap<IngredientCreateDto, IngredientEntity>();
            CreateMap<IngredientUpdateDto, IngredientEntity>();
            CreateMap<IngredientEntity, IngredientUpdateDto>();
        }
    }
}
