using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyFood.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Pass the request to the next middleware in the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception using the provided logger
                _logger.LogError(ex, "An unhandled exception occurred.");

                // Set the HTTP status code to 500 and content type to JSON
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                // Create a custom error response
                var errorResponse = new { Message = "An error occurred while processing your request." };
                var errorJson = JsonSerializer.Serialize(errorResponse);

                await context.Response.WriteAsync(errorJson);
            }
        }
    }
}
