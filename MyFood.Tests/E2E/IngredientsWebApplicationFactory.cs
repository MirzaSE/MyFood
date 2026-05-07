using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MyFood.Tests.E2E
{
    /// <summary>
    /// Runs the API with <c>ASPNETCORE_ENVIRONMENT=Testing</c> so <see cref="Program"/> uses in-memory EF + bootstrap seed.
    /// </summary>
    public sealed class IngredientsWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
        }
    }
}
