using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Api.MappingProfiles
{
    public class FoodMappings : Profile
    {
        public FoodMappings()
        {
            CreateMap<FoodEntity, FoodDto>()
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.FoodIngredients));
            CreateMap<FoodDto, FoodEntity>();
            CreateMap<FoodEntity, FoodUpdateDto>().ReverseMap();
            CreateMap<FoodEntity, FoodCreateDto>().ReverseMap();
            CreateMap<FoodIngredientEntity, FoodIngredientDto>()
                .ForMember(dest => dest.IngredientId, opt => opt.MapFrom(src => src.IngredientEntityId))
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src => src.Ingredient != null ? src.Ingredient.Name : ""))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Ingredient != null ? src.Ingredient.Unit : ""))
                .ForMember(dest => dest.CaloriesPerUnit, opt => opt.MapFrom(src => src.Ingredient != null ? src.Ingredient.CaloriesPerUnit : 0))
                .ForMember(dest => dest.Protein, opt => opt.MapFrom(src => src.Ingredient != null ? src.Ingredient.Protein : 0))
                .ForMember(dest => dest.Carbs, opt => opt.MapFrom(src => src.Ingredient != null ? src.Ingredient.Carbs : 0))
                .ForMember(dest => dest.Fat, opt => opt.MapFrom(src => src.Ingredient != null ? src.Ingredient.Fat : 0));
        }
    }
}
