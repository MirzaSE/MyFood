using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
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

                seedDataService.Initialize(dbContext);
            }
        }
    }
}
