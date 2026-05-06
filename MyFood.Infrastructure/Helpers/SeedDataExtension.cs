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
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FoodDbContext>();
                var seedDataService = scope.ServiceProvider.GetRequiredService<ISeedDataService>();
                var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedData");

                try
                {
                    if (!dbContext.Database.CanConnect())
                    {
                        logger.LogWarning("Skipping database seeding because the database is not reachable.");
                        return;
                    }

                    seedDataService.Initialize(dbContext);
                }
                catch (DbUpdateException ex)
                {
                    logger.LogWarning(ex, "Skipping database seeding due to a database update error.");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Skipping database seeding due to an unexpected startup error.");
                }
            }
        }
    }
}
