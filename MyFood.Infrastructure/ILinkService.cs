using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MyFood.Application;

namespace MyFood.Infrastructure
{
    public interface ILinkService<T>
    {
        object ExpandSingleFoodItem(object resource, int identifier, ApiVersion version, IUrlHelper urlHelper);

        List<LinkDto> CreateLinksForCollection(QueryParameters queryParameters, int totalCount, ApiVersion version, IUrlHelper urlHelper);
    }
}