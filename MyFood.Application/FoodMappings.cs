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
            CreateMap<IngredientEntity, IngredientCreateDto>().ReverseMap();
            CreateMap<IngredientEntity, IngredientUpdateDto>().ReverseMap();
            CreateMap<FoodIngredientEntity, FoodIngredientDto>()
                .ForMember(dest => dest.IngredientId, opt => opt.MapFrom(src => src.IngredientEntityId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Ingredient.Name))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Ingredient.Unit))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.CaloriesPerUnit, opt => opt.MapFrom(src => src.Ingredient.CaloriesPerUnit))
                .ForMember(dest => dest.Calories, opt => opt.MapFrom(src => src.Quantity * src.Ingredient.CaloriesPerUnit))
                .ForMember(dest => dest.Protein, opt => opt.MapFrom(src => src.Quantity * src.Ingredient.Protein))
                .ForMember(dest => dest.Carbs, opt => opt.MapFrom(src => src.Quantity * src.Ingredient.Carbs))
                .ForMember(dest => dest.Fat, opt => opt.MapFrom(src => src.Quantity * src.Ingredient.Fat));
        }
    }
}
