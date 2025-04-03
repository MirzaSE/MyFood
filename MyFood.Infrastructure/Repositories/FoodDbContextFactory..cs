using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MyFood.Infrastructure.Repositories
{
    public class FoodDbContextFactory : IDesignTimeDbContextFactory<FoodDbContext>
    {
        public FoodDbContext CreateDbContext(string[] args)
        {
            // Build configuration from appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<FoodDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);

            return new FoodDbContext(optionsBuilder.Options);
        }
    }
}
