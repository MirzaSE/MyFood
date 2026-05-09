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
            CreateMap<IngredientEntity, IngredientDto>().ReverseMap();
            CreateMap<IngredientEntity, IngredientUpdateDto>().ReverseMap();
            CreateMap<IngredientEntity, IngredientCreateDto>().ReverseMap();
        }
    }
}
