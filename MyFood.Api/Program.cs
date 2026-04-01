using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyFood.Api;
using MyFood.Api.MappingProfiles;
using MyFood.Api.Middleware;
using MyFood.Api.Services;
<<<<<<< HEAD
using MyFood.Domain.Entities;
=======
using MyFood.Application.Entities;
<<<<<<< HEAD
>>>>>>> a138ff7 (Add JWT authentication setup)
=======
using MyFood.Application.Repositories;
using MyFood.Application.Services;
>>>>>>> 3bdfcc1 (Assignment 3 done)
using MyFood.Infrastructure;
using MyFood.Infrastructure.Helpers;
using MyFood.Infrastructure.Repositories;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
<<<<<<< HEAD
=======
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
>>>>>>> 5f86e13 (Add middleware and authentication)
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
<<<<<<< HEAD
builder.WebHost.UseUrls("http://*:8080");
=======

builder.Services.AddScoped<IIngredientRepository, IngredientSqlRepository>(); 

>>>>>>> d243566 (Assignment 1 - Ingredients API)
builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                       options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCustomCors("AllowAllOrigins");

builder.Services.AddSingleton<ISeedDataService, SeedDataService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVerificationTokenService, VerificationTokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IFoodRepository, FoodSqlRepository>();
builder.Services.AddScoped<IIngredientRepository, IngredientSqlRepository>();
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
           b => b.MigrationsAssembly("MyFood.Infrastructure")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
<<<<<<< HEAD
               .AddEntityFrameworkStores<FoodDbContext>()
               .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });
=======
    .AddEntityFrameworkStores<FoodDbContext>()
    .AddDefaultTokenProviders();

>>>>>>> a138ff7 (Add JWT authentication setup)

builder.Services.AddAutoMapper(typeof(FoodMappings), typeof(IngredientMappings));

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT key is missing in configuration."));
var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT issuer is missing in configuration.");
var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT audience is missing in configuration.");

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
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };

    options.IncludeErrorDetails = true;
});

var app = builder.Build();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

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

    app.SeedData();
}
else
{
    app.AddProductionExceptionHandling(loggerFactory);
}

<<<<<<< HEAD

app.UseCors("AllowAllOrigins");
//app.UseHttpsRedirection(); // can cause docker issue


=======
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

//Add support to logging request with SERILOG
app.UseSerilogRequestLogging();

app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();

>>>>>>> 5f86e13 (Add middleware and authentication)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
