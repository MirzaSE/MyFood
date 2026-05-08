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
            CreateMap<FoodCreateDto, FoodEntity>();
            CreateMap<FoodUpdateDto, FoodEntity>();

            CreateMap<IngredientEntity, IngredientDto>().ReverseMap();
            CreateMap<CreateIngredientDto, IngredientEntity>();
            CreateMap<UpdateIngredientDto, IngredientEntity>();
        }
    }
}