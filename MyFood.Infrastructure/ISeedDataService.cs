using MyFood.Infrastructure.Repositories;

namespace MyFood.Api.Services
{
    public interface ISeedDataService
    {
        void Initialize(FoodDbContext context);
    }
}
