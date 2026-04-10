using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyFood.Infrastructure.Repositories
{
    public class FoodDbContextFactory : IDesignTimeDbContextFactory<FoodDbContext>
    {
        public FoodDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FoodDbContext>();
            optionsBuilder.UseSqlServer(
                "Data Source=localhost;Initial Catalog=MyFood_Database;User Id=SA; Password=Password1!;Connect Timeout=30;TrustServerCertificate=True",
                builder => builder.MigrationsAssembly("MyFood.Infrastructure"));

            return new FoodDbContext(optionsBuilder.Options);
        }
    }
}