using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using MyFood.Api.Services;
using MyFood.Application;
using MyFood.Infrastructure.Helpers;
using MyFood.Infrastructure.Models;
using System.Reflection;

namespace MyFood.Infrastructure
{
    public class LinkService<T> : ILinkService<T>
    {
        public LinkService()
        {
        }

        public List<LinkDto> CreateLinksForCollection(QueryParameters queryParameters, int totalCount, ApiVersion version, IUrlHelper urlHelper)
        {
            Type controllerType = (typeof(T));
            MethodInfo[] methods = controllerType.GetMethods();

            var links = new List<LinkDto>();
            var getAllMethodName = GetMethod(methods, typeof(HttpGetAttribute), 0);

            var selfUrl = urlHelper.Link(getAllMethodName, new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.Page,
                orderby = queryParameters.OrderBy
            });
            if (!string.IsNullOrEmpty(selfUrl))
            {
                links.Add(new LinkDto(selfUrl, "self", "GET"));
            }

            var firstUrl = urlHelper.Link(getAllMethodName, new
            {
                pagecount = queryParameters.PageCount,
                page = 1,
                orderby = queryParameters.OrderBy
            });
            if (!string.IsNullOrEmpty(firstUrl))
            {
                links.Add(new LinkDto(firstUrl, "first", "GET"));
            }

            var lastUrl = urlHelper.Link(getAllMethodName, new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.GetTotalPages(totalCount),
                orderby = queryParameters.OrderBy
            });
            if (!string.IsNullOrEmpty(lastUrl))
            {
                links.Add(new LinkDto(lastUrl, "last", "GET"));
            }

            if (queryParameters.HasNext(totalCount))
            {
                var nextUrl = urlHelper.Link(getAllMethodName, new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page + 1,
                    orderby = queryParameters.OrderBy
                });
                if (!string.IsNullOrEmpty(nextUrl))
                {
                    links.Add(new LinkDto(nextUrl, "next", "GET"));
                }
            }

            if (queryParameters.HasPrevious())
            {
                var previousUrl = urlHelper.Link(getAllMethodName, new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page - 1,
                    orderby = queryParameters.OrderBy
                });
                if (!string.IsNullOrEmpty(previousUrl))
                {
                    links.Add(new LinkDto(previousUrl, "previous", "GET"));
                }
            }

            var posturl = urlHelper.Link(GetMethod(methods, typeof(HttpPostAttribute)), new { version = version.ToString() });
            if (!string.IsNullOrEmpty(posturl))
            {
                links.Add(new LinkDto(posturl, "create", "POST"));
            }

            return links;
        }

        public object ExpandSingleFoodItem(object resource, int identifier, ApiVersion version, IUrlHelper urlHelper)
        {
            var resourceToReturn = resource.ToDynamic() as IDictionary<string, object>;
            if (resourceToReturn == null)
            {
                // If conversion fails, return the original resource
                return resource;
            } 

            var links = GetLinksForSingleItem(identifier, version, urlHelper);
            resourceToReturn.Add("links", links);
            return resourceToReturn;
        }

        private IEnumerable<LinkDto> GetLinksForSingleItem(int id, ApiVersion version, IUrlHelper urlHelper)
        {
            Type myType = (typeof(T));
            MethodInfo[] methods = myType.GetMethods();
            var links = new List<LinkDto>();

            var getLink = urlHelper.Link(GetMethod(methods, typeof(HttpGetAttribute), 1), new { version = version.ToString(), id = id });
            if (!string.IsNullOrEmpty(getLink))
            {
                links.Add(new LinkDto(getLink, "self", "GET"));
            }

            var deleteLink = urlHelper.Link(GetMethod(methods, typeof(HttpDeleteAttribute)), new { version = version.ToString(), id = id });
            if (!string.IsNullOrEmpty(deleteLink))
            {
                links.Add(new LinkDto(deleteLink, "delete", "DELETE"));
            }

            var createLink = urlHelper.Link(GetMethod(methods, typeof(HttpPostAttribute)), new { version = version.ToString() });
            if (!string.IsNullOrEmpty(createLink))
            {
                links.Add(new LinkDto(createLink, "create_food", "POST"));
            }

            var updateLink = urlHelper.Link(GetMethod(methods, typeof(HttpPutAttribute)), new { version = version.ToString(), id = id });
            if (!string.IsNullOrEmpty(updateLink))
            {
                links.Add(new LinkDto(updateLink, "update_food", "PUT"));
            }

            return links;
        }

        private string GetMethod(MethodInfo[] methods, Type type, int routeParamsLength = 0)
        {
            var filteredMethods = methods.Where(m => m.GetCustomAttributes(type, false).Length > 0).ToArray();

            if (filteredMethods.Length == 0) return "";

            if (routeParamsLength == 0)
            {
                var toReturn = filteredMethods.FirstOrDefault();
                return toReturn is not null ? toReturn.Name : "";
            }

            foreach (var method in filteredMethods)
            {
                var routeAttribs = method.GetCustomAttributes(typeof(Microsoft.AspNetCore.Components.RouteAttribute));
                if (routeAttribs.Count() == routeParamsLength)
                    return method.Name;
            }

            return "";
        }
    }
}