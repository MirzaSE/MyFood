using AutoMapper;
using MyFood.Application.Dtos;

using MyFood.Domain.Entities;
namespace MyFood.Api.MappingProfiles
{
    public class FoodMappings : Profile
    {
        public FoodMappings()
        {
            // Mapping from Entity to DTO
            CreateMap<FoodEntity, FoodDto>()
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));

            CreateMap<IngredientEntity, IngredientDto>();

            // Mapping from DTO to Entity for creation/updating
            CreateMap<FoodCreateDto, FoodEntity>()
                .ForMember(dest => dest.Ingredients, opt => opt.Ignore()); // <-- ignore circular nav property

            CreateMap<FoodUpdateDto, FoodEntity>()
                .ForMember(dest => dest.Ingredients, opt => opt.Ignore());

            CreateMap<IngredientDto, IngredientEntity>();
        }
    }
}
