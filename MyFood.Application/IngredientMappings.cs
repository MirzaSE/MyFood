using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;


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
