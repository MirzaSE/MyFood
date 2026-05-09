using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyFood.Api;
using MyFood.Api.Services;
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

        public async Task InitializeAsync()
        {
            var databaseName = $"MyFoodTests-{Guid.NewGuid()}";
            Factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<DbContextOptions<FoodDbContext>>();
                        services.AddDbContext<FoodDbContext>(options =>
                            options.UseInMemoryDatabase(databaseName));
                    });
                });

            Client = Factory.CreateClient();

            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FoodDbContext>();
            var seedDataService = scope.ServiceProvider.GetRequiredService<ISeedDataService>();
            dbContext.Database.EnsureCreated();
            seedDataService.Initialize(dbContext);
        }

        public async Task DisposeAsync()
        {
            Client?.Dispose();
            Factory?.Dispose();
        }

        protected async Task<string> RegisterAndLogin(string username = "testuser", string password = "Test@123")
        {
            var registerModel = new { username, email = $"{username}@example.com", password };
            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerModel),
                Encoding.UTF8,
                "application/json");

            var registerResponse = await Client.PostAsync("/api/authenticate/register", registerContent);
            if (registerResponse.IsSuccessStatusCode)
            {
                var registerBody = await registerResponse.Content.ReadAsStringAsync();
                using var registerDoc = JsonDocument.Parse(registerBody);
                if (registerDoc.RootElement.TryGetProperty("token", out var registerToken))
                {
                    return registerToken.GetString() ?? string.Empty;
                }
            }

            var loginModel = new { username, password };
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

            var loginBody = await loginResponse.Content.ReadAsStringAsync();
            throw new Exception($"Failed to obtain authentication token. RegisterStatus={registerResponse.StatusCode}, LoginStatus={loginResponse.StatusCode}, LoginBody={loginBody}");
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
