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
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedData");

            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FoodDbContext>();
                var seedDataService = scope.ServiceProvider.GetRequiredService<ISeedDataService>();

                if (!dbContext.Database.CanConnect())
                {
                    logger.LogWarning("Skipping seed data because SQL Server is not reachable.");
                    return;
                }

                seedDataService.Initialize(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Skipping seed data due to a database startup error.");
            }
        }
    }
}
