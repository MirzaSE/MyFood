using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Api.MappingProfiles
{
    public class IngredientMappings : Profile
    {
        public IngredientMappings()
        {
            CreateMap<IngredientEntity, IngredientDto>().ReverseMap();
            CreateMap<IngredientEntity, UpdateIngredientDto>().ReverseMap();
            CreateMap<IngredientEntity, CreateIngredientDto>().ReverseMap();
        }
    }
}