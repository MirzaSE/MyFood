using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MyFood.Domain.Interfaces;

namespace MyFood.Application.Helpers
{
    public static class SeedDataExtension
    {
        public static void SeedData(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var foodRepository = scope.ServiceProvider.GetRequiredService<IFoodRepository>();
                var seedDataService = scope.ServiceProvider.GetRequiredService<ISeedDataService>();

                seedDataService.Initialize(foodRepository);
            }
        }
    }
}
