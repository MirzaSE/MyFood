using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;

namespace MyFood.Api.MappingProfiles
{
    public class FoodMappings : Profile
    {
        public FoodMappings()
        {
            // Food Mappings
            CreateMap<FoodEntity, FoodDto>().ReverseMap();
            CreateMap<FoodEntity, FoodUpdateDto>().ReverseMap();
            CreateMap<FoodEntity, FoodCreateDto>().ReverseMap();

            // Ingredient Mappings - ADD THESE THREE LINES
            CreateMap<IngredientEntity, IngredientDto>().ReverseMap();
            CreateMap<IngredientEntity, IngredientCreateDto>().ReverseMap();
            CreateMap<IngredientEntity, IngredientUpdateDto>().ReverseMap();
        }
    }
}
