using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using MyFood.Api;
using MyFood.Api.MappingProfiles;
using MyFood.Api.Services;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Repositories;
using MyFood.Infrastructure.Helpers;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/app-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Policy
builder.Services.AddCustomCors("AllowAllOrigins");

// Dependency Injection
builder.Services.AddSingleton<ISeedDataService, SeedDataService>();
builder.Services.AddScoped<IFoodRepository, FoodSqlRepository>();
builder.Services.AddScoped(typeof(ILinkService<>), typeof(LinkService<>));
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();

// Enable Lowercase URLs
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddVersioning();

// Database Connection
builder.Services.AddDbContext<FoodDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("MyFood.Infrastructure")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(FoodMappings));

// Authentication & Authorization (Assuming JWT Authentication is needed)
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.Authority = "https://your-auth-provider.com"; // Replace with actual authority
        options.Audience = "myfood-api";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

// Middleware
app.UseMiddleware<MyFood.Api.Middleware.ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseMiddleware<MyFood.Api.Middleware.RequestLoggingMiddleware>();

app.UseAuthentication(); // Ensure authentication middleware is added
app.UseAuthorization();

// Swagger Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
    
    app.SeedData();
}
else
{
    app.AddProductionExceptionHandling(loggerFactory);
}

// Map Controllers
app.MapControllers();

// Run the application
app.Run();
