using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;

namespace MyFood.Api.MappingProfiles
{
    public class FoodMappings : Profile
    {
        public FoodMappings()
        {
            CreateMap<FoodEntity, FoodDto>().ReverseMap();
            CreateMap<FoodEntity, FoodUpdateDto>().ReverseMap();
            CreateMap<FoodEntity, FoodCreateDto>().ReverseMap();
            CreateMap<IngredientEntity, IngredientDto>().ReverseMap();
            CreateMap<IngredientCreateDto, IngredientEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
