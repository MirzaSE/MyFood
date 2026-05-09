using AutoMapper;
using MyFood.Application.Dtos;
using MyFood.Domain.Entities;

namespace MyFood.Application;

public class IngredientMappings : Profile
{
    public IngredientMappings()
    {
        CreateMap<IngredientEntity, IngredientDto>().ReverseMap();
        CreateMap<IngredientEntity, IngredientCreateDto>().ReverseMap();
        CreateMap<IngredientUpdateDto, IngredientEntity>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
