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

            // Ingredient
            CreateMap<IngredientEntity, IngredientDto>().ReverseMap();
            CreateMap<IngredientCreateDto, IngredientEntity>();
            // For partial updates, skip nulls so unset properties don't overwrite the entity
            CreateMap<IngredientUpdateDto, IngredientEntity>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
