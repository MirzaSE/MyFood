using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MyFood.Infrastructure.Repositories;

namespace MyFood.Infrastructure.Repositories
{
    public class FoodDbContextFactory : IDesignTimeDbContextFactory<FoodDbContext>
    {
        public FoodDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FoodDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=MyFood_Database;User Id=SA;Password=Password1!;TrustServerCertificate=True;");

            return new FoodDbContext(optionsBuilder.Options);
        }
    }
}
