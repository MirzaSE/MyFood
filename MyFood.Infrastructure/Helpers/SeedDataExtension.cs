using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyFood.Api.Services;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Infrastructure.Helpers
{
    public static class SeedDataExtension
    {
        public static void SeedData(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<FoodDbContext>();
            var seedDataService = scope.ServiceProvider.GetRequiredService<ISeedDataService>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<FoodDbContext>>();

            try
            {
                dbContext.Database.Migrate();

                if (!dbContext.Database.CanConnect())
                {
                    logger.LogWarning("Skipping seed because the database connection is not available.");
                    return;
                }

                seedDataService.Initialize(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database seeding failed during application startup.");
            }
        }
    }
}
