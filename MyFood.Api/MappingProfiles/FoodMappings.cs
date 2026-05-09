using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Api.MappingProfiles
{
    public class FoodMappings : Profile
    {
        public FoodMappings()
        {
            CreateMap<FoodEntity, FoodDto>().ReverseMap();
            CreateMap<FoodEntity, FoodUpdateDto>().ReverseMap();
            CreateMap<FoodEntity, FoodCreateDto>().ReverseMap();

            CreateMap<IngredientEntity, IngredientDto>()
                .ForMember(dest => dest.FoodId, opt => opt.MapFrom(src => src.FoodEntityId))
                .ReverseMap()
                .ForMember(dest => dest.FoodEntityId, opt => opt.MapFrom(src => src.FoodId));

            CreateMap<IngredientCreateDto, IngredientEntity>()
                .ForMember(dest => dest.FoodEntityId, opt => opt.MapFrom(src => src.FoodId));
        }
    }
}
