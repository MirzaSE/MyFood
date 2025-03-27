using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MyFood.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log incoming request details
            _logger.LogInformation($"Incoming request: {context.Request.Method} {context.Request.Path}");

            // Call the next middleware in the pipeline
            await _next(context);

            // Log outgoing response status
            _logger.LogInformation($"Outgoing response: {context.Response.StatusCode}");
        }
    }
}
