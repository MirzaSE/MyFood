using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyFood.Api.Services;
using MyFood.Application;
using MyFood.Infrastructure.Helpers;
using MyFood.Infrastructure.Models;
using System.Reflection;

namespace MyFood.Infrastructure
{
    public class LinkService<T> : ILinkService<T>
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LinkService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? LinkByRouteName(string routeName, object? values = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is null)
            {
                return null;
            }

            return _linkGenerator.GetUriByRouteValues(
                httpContext,
                routeName,
                values is null ? null : new RouteValueDictionary(values));
        }

        public List<LinkDto> CreateLinksForCollection(QueryParameters queryParameters, int totalCount, ApiVersion version)
        {
            Type controllerType = (typeof(T));
            MethodInfo[] methods = controllerType.GetMethods();

            var links = new List<LinkDto>();
            var getAllMethodName = GetMethod(methods, typeof(HttpGetAttribute), 0);

            // self 
            links.Add(new LinkDto(LinkByRouteName(getAllMethodName, new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.Page,
                orderby = queryParameters.OrderBy
            }) ?? string.Empty, "self", "GET"));

            links.Add(new LinkDto(LinkByRouteName(getAllMethodName, new
            {
                pagecount = queryParameters.PageCount,
                page = 1,
                orderby = queryParameters.OrderBy
            }) ?? string.Empty, "first", "GET"));

            links.Add(new LinkDto(LinkByRouteName(getAllMethodName, new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.GetTotalPages(totalCount),
                orderby = queryParameters.OrderBy
            }) ?? string.Empty, "last", "GET"));

            if (queryParameters.HasNext(totalCount))
            {
                links.Add(new LinkDto(LinkByRouteName(getAllMethodName, new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page + 1,
                    orderby = queryParameters.OrderBy
                }) ?? string.Empty, "next", "GET"));
            }

            if (queryParameters.HasPrevious())
            {
                links.Add(new LinkDto(LinkByRouteName(getAllMethodName, new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page - 1,
                    orderby = queryParameters.OrderBy
                }) ?? string.Empty, "previous", "GET"));
            }

            var posturl = LinkByRouteName(GetMethod(methods, typeof(HttpPostAttribute)), new { version = version.ToString() }) ?? string.Empty;

            links.Add(
               new LinkDto(posturl,
               "create",
               "POST"));

            return links;
        }

        public object ExpandSingleFoodItem(object resource, int identifier, ApiVersion version)
        {
            var resourceToReturn = (IDictionary<string, object?>)resource.ToDynamic();

            var links = GetLinksForSingleItem(identifier, version);

            resourceToReturn["links"] = links;

            return resourceToReturn;
        }


        private IEnumerable<LinkDto> GetLinksForSingleItem(int id, ApiVersion version)
        {
            Type myType = (typeof(T));
            MethodInfo[] methods = myType.GetMethods();
            var links = new List<LinkDto>();

            var getLink = LinkByRouteName(GetMethod(methods, typeof(Microsoft.AspNetCore.Mvc.HttpGetAttribute), 1), new { version = version.ToString(), id = id }) ?? string.Empty;
            links.Add(new LinkDto(getLink, "self", "GET"));

            var deleteLink = LinkByRouteName(GetMethod(methods, typeof(Microsoft.AspNetCore.Mvc.HttpDeleteAttribute)), new { version = version.ToString(), id = id }) ?? string.Empty;
            links.Add(
              new LinkDto(deleteLink,
              "delete",
              "DELETE"));

            var createLink = LinkByRouteName(GetMethod(methods, typeof(HttpPostAttribute)), new { version = version.ToString() }) ?? string.Empty;
            links.Add(
              new LinkDto(createLink,
              "create_food",
              "POST"));

            var updateLink = LinkByRouteName(GetMethod(methods, typeof(Microsoft.AspNetCore.Mvc.HttpPutAttribute)), new { version = version.ToString(), id = id }) ?? string.Empty;
            links.Add(
               new LinkDto(updateLink,
               "update_food",
               "PUT"));

            return links;
        }

        private string GetMethod(MethodInfo[] methods, Type type, int routeParamsLength = 0)
        {
            var filteredMethods = methods.Where(m => m.GetCustomAttributes(type, false).Length > 0).ToArray();

            if (filteredMethods.Length == 0)
            {
                return "";
            }

            if (routeParamsLength == 0)
            {
                var toReturn = filteredMethods.FirstOrDefault();

                return toReturn is not null ? toReturn.Name : "";
            }

            foreach (var method in filteredMethods)
            {
                var routeAttribs = method.GetCustomAttributes(typeof(Microsoft.AspNetCore.Components.RouteAttribute));

                if (routeAttribs.Count() == routeParamsLength)
                {
                    return method.Name;
                }
            }

            return "";
        }
    }
}
