namespace MyFood.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occured: {Method} {Path}", 
                    context.Request.Method, 
                    context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "An internal server error occurred. Please try again later.",
                    Detail = _env.IsDevelopment() ? ex.Message : null,
                    StackTrace = _env.IsDevelopment() ? ex.StackTrace : null
                };

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
