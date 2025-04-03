using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyFood.Api;
using MyFood.Api.MappingProfiles;
using MyFood.Api.Middleware;
using MyFood.Api.Services;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Helpers;
using MyFood.Infrastructure.Repositories;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.Text;
using YourProjectNamespace.Models;
using MyFood.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure HTTP/HTTPS based on environment
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://localhost:5000");
}

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
builder.Services.AddScoped(typeof(ILinkService<>), typeof(LinkService<>));
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddVersioning();

// -----------------------------------------------------------------------------
// JWT and Identity Configuration (Added)
// -----------------------------------------------------------------------------
// Read JWT settings directly from configuration and register the section.
var jwtSettings = builder.Configuration.GetSection("JWT").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));

// Add DbContext for FoodDbContext (using MyFood_Database connection)
builder.Services.AddDbContext<FoodDbContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("MyFood.Infrastructure")));

// Add Identity (ApplicationUser and IdentityRole) with FoodDbContext.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<FoodDbContext>()
    .AddDefaultTokenProviders();

// Add Razor Pages support (if needed)
builder.Services.AddRazorPages();

// -----------------------------------------------------------------------------
// JWT Authentication Configuration (Using settings from configuration)
// -----------------------------------------------------------------------------
#pragma warning disable CS8602 // Dereference of a possibly null reference.
var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.ValidIssuer,
        ValidAudience = jwtSettings.ValidAudience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    options.IncludeErrorDetails = true;

    // Only require HTTPS for JWT in production
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
});

// -----------------------------------------------------------------------------
// Other Service Registrations
// -----------------------------------------------------------------------------
builder.Services.AddAutoMapper(typeof(FoodMappings));

// Add support for logging with Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });

    app.SeedData();
}
else
{
    app.AddProductionExceptionHandling(loggerFactory);
    // Only use HTTPS redirection in production
    app.UseHttpsRedirection();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// Add support for logging requests with Serilog
app.UseSerilogRequestLogging();

app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages(); // If you are using Razor Pages

app.Run();
