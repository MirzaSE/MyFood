using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyFood.Api;
using MyFood.Infrastructure.Repositories;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    public class ApiE2ETestBase : IAsyncLifetime
    {
        protected WebApplicationFactory<Program> Factory { get; private set; } = null!;
        protected HttpClient Client { get; private set; } = null!;
        protected string? AuthToken { get; set; }

        public Task InitializeAsync()
        {
            var databaseName = $"MyFoodE2ETestDb_{Guid.NewGuid()}";

            Factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");

                    builder.ConfigureTestServices(services =>
                    {
                        var descriptorsToRemove = services
                            .Where(d =>
                                d.ServiceType == typeof(DbContextOptions<FoodDbContext>) ||
                                d.ServiceType == typeof(DbContextOptions) ||
                                (d.ServiceType.FullName != null &&
                                 d.ServiceType.FullName.Contains("IDbContextOptionsConfiguration")))
                            .ToList();

                        foreach (var descriptor in descriptorsToRemove)
                        {
                            services.Remove(descriptor);
                        }

                        services.AddDbContext<FoodDbContext>(options =>
                        {
                            options.UseInMemoryDatabase(databaseName);
                        });
                    });
                });

            Client = Factory.CreateClient();
            Client.BaseAddress = new Uri("http://localhost");

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            Client?.Dispose();
            Factory?.Dispose();

            return Task.CompletedTask;
        }

        protected async Task<string> RegisterAndLogin(
            string username = "testuser",
            string password = "Test@123")
        {
            var uniqueUsername = $"{username}_{Guid.NewGuid():N}";

            var registerModel = new
            {
                username = uniqueUsername,
                password
            };

            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerModel),
                Encoding.UTF8,
                "application/json");

            await Client.PostAsync("/api/authenticate/register", registerContent);

            var loginModel = new
            {
                username = uniqueUsername,
                password
            };

            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginModel),
                Encoding.UTF8,
                "application/json");

            var loginResponse = await Client.PostAsync("/api/authenticate/login", loginContent);

            if (loginResponse.IsSuccessStatusCode)
            {
                var responseBody = await loginResponse.Content.ReadAsStringAsync();

                using var jsonDoc = JsonDocument.Parse(responseBody);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("token", out var tokenElement))
                {
                    return tokenElement.GetString() ?? string.Empty;
                }
            }

            throw new Exception("Failed to obtain authentication token");
        }

        protected void SetAuthorizationToken(string token)
        {
            AuthToken = token;
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected void ClearAuthorizationToken()
        {
            AuthToken = null;
            Client.DefaultRequestHeaders.Authorization = null;
        }
    }
}