using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Api.MappingProfiles
{
    public class FoodMappings : Profile
    {
        public FoodMappings()
        {
            // Food
            CreateMap<FoodEntity, FoodDto>().ReverseMap();
            CreateMap<FoodEntity, FoodUpdateDto>().ReverseMap();
            CreateMap<FoodEntity, FoodCreateDto>().ReverseMap();
            CreateMap<FoodCreateDto, FoodEntity>();

            // Ingredient
            CreateMap<IngredientEntity, IngredientDto>().ReverseMap();
            CreateMap<IngredientCreateDto, IngredientEntity>();
            CreateMap<IngredientUpdateDto, IngredientEntity>()
                // Partial update: skip null members so PUT is safely partial.
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
