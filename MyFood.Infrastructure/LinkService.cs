using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using MyFood.Application.Models; // Use the merged namespace
using MyFood.Infrastructure.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyFood.Infrastructure
{
    public class LinkService<T> : ILinkService<T>
    {
        private readonly IUrlHelper _urlHelper;

        public LinkService(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        public List<LinkDto> CreateLinksForCollection(QueryParameters queryParameters, int totalCount, ApiVersion version)
        {
            Type controllerType = typeof(T);
            MethodInfo[] methods = controllerType.GetMethods();

            var links = new List<LinkDto>();
            var getAllMethodName = GetMethod(methods, typeof(HttpGetAttribute), 0);

            // Self link
            links.Add(new LinkDto
            {
                Href = _urlHelper.Link(getAllMethodName, new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page,
                    orderby = queryParameters.OrderBy
                }),
                Rel = "self",
                Method = "GET"
            });

            // First page link
            links.Add(new LinkDto
            {
                Href = _urlHelper.Link(getAllMethodName, new
                {
                    pagecount = queryParameters.PageCount,
                    page = 1,
                    orderby = queryParameters.OrderBy
                }),
                Rel = "first",
                Method = "GET"
            });

            // Last page link
            links.Add(new LinkDto
            {
                Href = _urlHelper.Link(getAllMethodName, new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.GetTotalPages(totalCount),
                    orderby = queryParameters.OrderBy
                }),
                Rel = "last",
                Method = "GET"
            });

            // Next page link
            if (queryParameters.HasNext(totalCount))
            {
                links.Add(new LinkDto
                {
                    Href = _urlHelper.Link(getAllMethodName, new
                    {
                        pagecount = queryParameters.PageCount,
                        page = queryParameters.Page + 1,
                        orderby = queryParameters.OrderBy
                    }),
                    Rel = "next",
                    Method = "GET"
                });
            }

            // Previous page link
            if (queryParameters.HasPrevious())
            {
                links.Add(new LinkDto
                {
                    Href = _urlHelper.Link(getAllMethodName, new
                    {
                        pagecount = queryParameters.PageCount,
                        page = queryParameters.Page - 1,
                        orderby = queryParameters.OrderBy
                    }),
                    Rel = "previous",
                    Method = "GET"
                });
            }

            // Create link
            var postUrl = _urlHelper.Link(GetMethod(methods, typeof(HttpPostAttribute)), new { version = version.ToString() });
            links.Add(new LinkDto
            {
                Href = postUrl,
                Rel = "create",
                Method = "POST"
            });

            return links;
        }

        public object ExpandSingleFoodItem(object resource, int identifier, ApiVersion version)
        {
            var resourceToReturn = resource.ToDynamic() as IDictionary<string, object>;

            var links = GetLinksForSingleItem(identifier, version);
            resourceToReturn.Add("links", links);

            return resourceToReturn;
        }

        private IEnumerable<LinkDto> GetLinksForSingleItem(int id, ApiVersion version)
        {
            Type myType = typeof(T);
            MethodInfo[] methods = myType.GetMethods();
            var links = new List<LinkDto>();

            // Self link
            var getLink = _urlHelper.Link(GetMethod(methods, typeof(HttpGetAttribute), 1), new { version = version.ToString(), id = id });
            links.Add(new LinkDto
            {
                Href = getLink,
                Rel = "self",
                Method = "GET"
            });

            // Delete link
            var deleteLink = _urlHelper.Link(GetMethod(methods, typeof(HttpDeleteAttribute)), new { version = version.ToString(), id = id });
            links.Add(new LinkDto
            {
                Href = deleteLink,
                Rel = "delete",
                Method = "DELETE"
            });

            // Create link
            var createLink = _urlHelper.Link(GetMethod(methods, typeof(HttpPostAttribute)), new { version = version.ToString() });
            links.Add(new LinkDto
            {
                Href = createLink,
                Rel = "create_food",
                Method = "POST"
            });

            // Update link
            var updateLink = _urlHelper.Link(GetMethod(methods, typeof(HttpPutAttribute)), new { version = version.ToString(), id = id });
            links.Add(new LinkDto
            {
                Href = updateLink,
                Rel = "update_food",
                Method = "PUT"
            });

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
                return toReturn?.Name ?? "";
            }

            foreach (var method in filteredMethods)
            {
                var routeAttribs = method.GetCustomAttributes(typeof(RouteAttribute));
                if (routeAttribs.Count() == routeParamsLength)
                {
                    return method.Name;
                }
            }

            return "";
        }
    }
}