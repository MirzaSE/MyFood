using Serilog;
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
using MyFood.Application.Repositories;
using MyFood.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog using settings from appsettings.json
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration); // Read Serilog settings from appsettings.json
});

// Add services to the container.

builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                       options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCustomCors("AllowAllOrigins");

builder.Services.AddSingleton<ISeedDataService, SeedDataService>();
builder.Services.AddScoped<IFoodRepository, FoodSqlRepository>();
builder.Services.AddScoped<IngredientSqlRepository>();
builder.Services.AddScoped<IIngredientRepository, IngredientSqlRepository>();
builder.Services.AddControllers();

builder.Services.AddScoped(typeof(ILinkService<>), typeof(LinkService<>));
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddVersioning();

builder.Services.AddDbContext<FoodDbContext>(opt =>
//opt.UseInMemoryDatabase("FoodDatabase"));
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("MyFood.Infrastructure") // Ensure migration assembly is correct
    ));
// If you want to use InMemoryDatabase for development, uncomment below:
// builder.Services.AddDbContext<FoodDbContext>(opt =>
//     opt.UseInMemoryDatabase("FoodDatabase"));

builder.Services.AddAutoMapper(typeof(FoodMappings));

var app = builder.Build();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

// Register the custom request logging middleware
app.UseMiddleware<RequestLoggingMiddleware>(); // Add this line to use your custom middleware

// Register the exception handling middleware (this should be after request logging)
app.UseMiddleware<ExceptionHandlingMiddleware>(); // Register your exception handling middleware

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        options =>
        {
            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }
        });

    app.SeedData(); // Ensure SeedData is correctly implemented if you intend to use it for testing or initial data
}
else
{
    app.AddProductionExceptionHandling(loggerFactory); // Define the Production Exception Handling if you use it.
}

// Enable request logging with Serilog (Optional: you can choose between this or the custom middleware)
app.UseSerilogRequestLogging(); // This logs all HTTP requests automatically

app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

