using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyFood.Api;
using MyFood.Api.MappingProfiles;
using MyFood.Api.Services;
using MyFood.Application.Repositories;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Helpers;
using MyFood.Infrastructure.Repositories;
using MyFood.Infrastructure.Services;
using Newtonsoft.Json.Serialization;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCustomCors("AllowAllOrigins");

// Services & Repositories
builder.Services.AddSingleton<ISeedDataService, SeedDataService>();
builder.Services.AddScoped<IFoodRepository, FoodSqlRepository>();
builder.Services.AddScoped<IIngredientRepository, IngredientSqlRepository>();
builder.Services.AddScoped(typeof(ILinkService<>), typeof(LinkService<>));
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();  // ← ADD THIS LINE
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVerificationTokenService, VerificationTokenService>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddVersioning();

// DbContext
builder.Services.AddDbContext<FoodDbContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("MyFood.Infrastructure")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(FoodMappings), typeof(IngredientMappings));

// Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<FoodDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(
    jwtSettings["Key"] ?? "FallbackKeyForTesting12345!");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured"),
            ValidAudience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured"),
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        options.IncludeErrorDetails = true;
    });

var app = builder.Build();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

// Configure HTTP pipeline
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
}

// app.UseMiddleware<ExceptionHandlingMiddleware>();
// app.UseMiddleware<RequestLoggingMiddleware>();

app.UseSerilogRequestLogging();

app.UseCors("AllowAllOrigins");
// app.UseHttpsRedirection(); // can cause docker issue

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();