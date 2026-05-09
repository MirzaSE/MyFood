using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Api.MappingProfiles
{
    public class FoodMappings : Profile
    {
        public FoodMappings()
        {
            CreateMap<FoodEntity, FoodDto>();
            CreateMap<FoodEntity, FoodUpdateDto>();
            CreateMap<FoodCreateDto, FoodEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.Ingredients, opt => opt.Ignore());
            CreateMap<FoodUpdateDto, FoodEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.Ingredients, opt => opt.Ignore());

            CreateMap<IngredientEntity, IngredientDto>();
            CreateMap<IngredientEntity, IngredientUpdateDto>();
            CreateMap<IngredientCreateDto, IngredientEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FoodEntity, opt => opt.Ignore())
                .ForMember(dest => dest.FoodEntityId, opt => opt.Ignore());
            CreateMap<IngredientUpdateDto, IngredientEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FoodEntity, opt => opt.Ignore())
                .ForMember(dest => dest.FoodEntityId, opt => opt.Ignore());
        }
    }
}
