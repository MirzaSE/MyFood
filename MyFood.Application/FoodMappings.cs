using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;

namespace MyFood.Api.MappingProfiles
{
    public class FoodMappings : Profile
    {
        public FoodMappings()
        {
            CreateMap<FoodEntity, FoodDto>()
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));

            CreateMap<IngredientEntity, IngredientDto>();
            CreateMap<FoodDto, FoodEntity>();
            CreateMap<IngredientDto, IngredientEntity>();
        }
    }
}
