using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using MyFood.Api;
using MyFood.Api.MappingProfiles;
using MyFood.Api.Services;
using MyFood.Infrastructure.Repositories;
using MyFood.Infrastructure.Helpers;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.EntityFrameworkCore;
using Serilog;
using MyFood.Api.Middleware;
using MyFood.Domain.Entities;
using MyFood.Infrastructure;
using MyFood.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IIngredientRepository, IngredientSqlRepository>();

builder.Services.AddCustomCors("AllowAllOrigins");

builder.Services.AddSingleton<ISeedDataService, SeedDataService>();
builder.Services.AddScoped<IFoodRepository, FoodSqlRepository>();
builder.Services.AddScoped(typeof(ILinkService<>), typeof(LinkService<>));
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddVersioning();

// Read JWT settings and bind to JwtSettings class
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));

// Configure the DbContext to use your FoodDbContext
builder.Services.AddDbContext<FoodDbContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("MyFood.Infrastructure")
              .EnableRetryOnFailure(
                  maxRetryCount: 5,
                  maxRetryDelay: TimeSpan.FromSeconds(30),
                  errorNumbersToAdd: null)));

// Add AutoMapper support
builder.Services.AddAutoMapper(typeof(FoodMappings));

// ***** Add Identity services using FoodDbContext *****
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<FoodDbContext>()
    .AddDefaultTokenProviders();

// (Optional) Add Razor Pages if your application uses them for login/registration UI.
builder.Services.AddRazorPages();

// Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

// Ensure roles exist on application startup
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedData.EnsureRolesExist(roleManager);
}

// Configure the HTTP request pipeline.
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

app.UseSerilogRequestLogging();
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseAuthorization();

app.MapControllers();

// If you added Razor Pages for login/registration UI, map them as well.
app.MapRazorPages();

app.Run();

// SeedData class to ensure roles exist
public static class SeedData
{
    public static async Task EnsureRolesExist(RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { "USER", "ADMIN" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}