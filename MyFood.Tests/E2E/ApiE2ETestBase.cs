using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using MyFood.Api;
using System.Net.Http.Headers;
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
            Factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                    builder.ConfigureAppConfiguration((_, config) =>
                    {
                        config.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            ["Testing:DatabaseName"] = $"MyFoodE2E-{Guid.NewGuid()}"
                        });
                    });
                });

            Client = Factory.CreateClient();
            
            // Use localhost with port 8080 as configured in Program.cs
            Client.BaseAddress = new Uri("http://localhost:8080");
        }

        public async Task DisposeAsync()
        {
            Client?.Dispose();
            Factory?.Dispose();
        }

        /// <summary>
        /// Helper method to register a new test user and obtain JWT token
        /// </summary>
        protected async Task<string> RegisterAndLogin(string username = "testuser", string password = "Test@123")
        {
            // Register
            var registerModel = new { username, email = $"{username}@example.com", password };
            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerModel),
                new MediaTypeHeaderValue("application/json"));

            await Client.PostAsync("/api/authenticate/register", registerContent);

            // Login
            var loginModel = new { username, password };
            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginModel),
                new MediaTypeHeaderValue("application/json"));

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

        /// <summary>
        /// Sets the Authorization header with Bearer token
        /// </summary>
        protected void SetAuthorizationToken(string token)
        {
            AuthToken = token;
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Clears the Authorization header
        /// </summary>
        protected void ClearAuthorizationToken()
        {
            AuthToken = null;
            Client.DefaultRequestHeaders.Authorization = null;
        }
    }
}
