using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application;

public class FoodMappings : Profile
{
    public FoodMappings()
    {
        CreateMap<FoodEntity, FoodDto>();
        CreateMap<FoodDto, FoodUpdateDto>();
        CreateMap<FoodCreateDto, FoodEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Created, opt => opt.Ignore())
            .ForMember(dest => dest.Ingredients, opt => opt.Ignore());
        CreateMap<FoodUpdateDto, FoodEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Created, opt => opt.Ignore())
            .ForMember(dest => dest.Ingredients, opt => opt.Ignore());
        CreateMap<IngredientEntity, IngredientDto>().ReverseMap();
        CreateMap<IngredientEntity, IngredientUpdateDto>().ReverseMap();
        CreateMap<IngredientEntity, IngredientCreateDto>().ReverseMap();
        CreateMap<IngredientDto, FoodIngredientDto>();
    }
}
