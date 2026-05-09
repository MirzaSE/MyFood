using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Api.MappingProfiles
{
    public class IngredientsMappings : Profile
    {
        public IngredientsMappings()
        {
            CreateMap<IngredientEntity, IngridientDto>().ReverseMap();
            CreateMap<IngredientEntity, IngridientUpdate>().ReverseMap();
            CreateMap<IngredientEntity, IngridientCreateDto>().ReverseMap();
        }
    }
}
