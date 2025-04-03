using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyFood.Api.Models;
using MyFood.Infrastructure;
using MyFood.Infrastructure.Repositories;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Validate JWT settings early
try
{
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
    var jwtSettings = builder.Configuration.GetSection("JWT").Get<JwtSettings>();
    if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Secret))
    {
        throw new InvalidOperationException("JWT configuration is missing or invalid");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"FATAL ERROR in JWT configuration: {ex.Message}");
    throw;
}

// Database Configuration with health check
try
{
    builder.Services.AddDbContext<FoodDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    });

    // Verify database connection early
    var tempProvider = builder.Services.BuildServiceProvider();
    using (var scope = tempProvider.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<FoodDbContext>();
        if (!await db.Database.CanConnectAsync())
        {
            throw new Exception("Cannot connect to database");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"FATAL DATABASE ERROR: {ex.Message}");
    throw;
}

// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<FoodDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication with validation
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.ValidIssuer,
        ValidAudience = jwtSettings.ValidAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ClockSkew = TimeSpan.Zero
    };
});

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Validation Error Handling
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage)
            .ToList();

        return new BadRequestObjectResult(new
        {
            Message = "Validation errors occurred",
            Errors = errors
        });
    };
});

// Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

    foreach (var description in provider.ApiVersionDescriptions)
    {
        c.SwaggerDoc(
            description.GroupName,
            new OpenApiInfo()
            {
                Title = $"MyFood API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "Includes detailed validation error responses"
            });
    }

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Global Error Handling Middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Unhandled exception");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An error occurred. Please try again later.");
    }
});

// Initialize Database and Roles
async Task InitializeDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<FoodDbContext>();

            // Apply pending migrations
            if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
            {
                app.Logger.LogInformation("Applying pending migrations...");
                await dbContext.Database.MigrateAsync();
            }

            // Seed roles
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in new[] { "Admin", "User" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role {role}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                    app.Logger.LogInformation("Created role: {Role}", role);
                }
            }

            // Seed admin user if in development
            if (app.Environment.IsDevelopment())
            {
                await SeedAdminUser(scope.ServiceProvider);
            }
        }
        catch (Exception ex)
        {
            app.Logger.LogCritical(ex, "Database initialization failed");
            throw;
        }
    }
}

async Task SeedAdminUser(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var config = serviceProvider.GetRequiredService<IConfiguration>();

    var adminEmail = config["AdminUser:Email"] ?? "admin@example.com";
    var adminPassword = config["AdminUser:Password"] ?? "Admin@1234";

    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (!result.Succeeded)
        {
            throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        await userManager.AddToRoleAsync(adminUser, "Admin");
        app.Logger.LogInformation("Admin user created successfully");
    }
}

try
{
    await InitializeDatabase();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Application startup failed");
    throw;
}

// Configure HTTP Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in app.Services.GetRequiredService<IApiVersionDescriptionProvider>().ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
        options.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();