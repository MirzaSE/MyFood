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
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Helpers;
using MyFood.Infrastructure.Repositories;
using Newtonsoft.Json.Serialization;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.UseUrls("http://*:8080");
builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                       options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCustomCors("AllowAllOrigins");

builder.Services.AddSingleton<ISeedDataService, SeedDataService>();
builder.Services.AddScoped<IFoodRepository, FoodSqlRepository>();
builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVerificationTokenService, VerificationTokenService>();
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


builder.Services.AddAutoMapper(typeof(FoodMappings));

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
    
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
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

builder.Services.AddAutoMapper(typeof(FoodMappings));

var app = builder.Build();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FoodDbContext>();

    await dbContext.Database.ExecuteSqlRawAsync(
        """
        IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
        BEGIN
            CREATE TABLE [__EFMigrationsHistory] (
                [MigrationId] nvarchar(150) NOT NULL,
                [ProductVersion] nvarchar(32) NOT NULL,
                CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
            );
        END

        IF COL_LENGTH('AspNetUsers', 'IsEmailVerified') IS NULL
            ALTER TABLE [AspNetUsers] ADD [IsEmailVerified] bit NOT NULL CONSTRAINT [DF_AspNetUsers_IsEmailVerified] DEFAULT(0);

        IF COL_LENGTH('AspNetUsers', 'VerificationToken') IS NULL
            ALTER TABLE [AspNetUsers] ADD [VerificationToken] nvarchar(max) NULL;

        IF COL_LENGTH('AspNetUsers', 'VerificationTokenExpiryUtc') IS NULL
            ALTER TABLE [AspNetUsers] ADD [VerificationTokenExpiryUtc] datetime2 NULL;

        IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250312150424_Initial')
            INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250312150424_Initial', N'10.0.5');

        IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250312162119_ReduceCharacters')
            INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250312162119_ReduceCharacters', N'10.0.5');

        IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260325112226_AuthenticationUser')
            INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260325112226_AuthenticationUser', N'10.0.5');

        IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260410140445_AddEmailVerificationToApplicationUser')
            INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260410140445_AddEmailVerificationToApplicationUser', N'10.0.5');
        """);
}

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
//app.UseMiddleware<ExceptionHandlingMiddleware>();
//app.UseMiddleware<RequestLoggingMiddleware>();

//Add support to logging request with SERILOG
app.UseSerilogRequestLogging();

app.UseCors("AllowAllOrigins");
//app.UseHttpsRedirection(); // can cause docker issue


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();