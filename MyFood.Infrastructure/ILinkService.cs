using Microsoft.AspNetCore.Mvc;
using MyFood.Application.Models; // Use the merged namespace

namespace MyFood.Infrastructure
{
    public interface ILinkService<T>
    {
        object ExpandSingleFoodItem(object resource, int identifier, ApiVersion version);
        List<LinkDto> CreateLinksForCollection(QueryParameters queryParameters, int totalCount, ApiVersion version);
    }
}