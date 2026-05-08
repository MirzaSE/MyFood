using Microsoft.AspNetCore.Mvc.Testing;
using MyFood.Api;
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
            Factory = new WebApplicationFactory<Program>();

            Client = Factory.CreateClient();

            Client.BaseAddress = new Uri("http://localhost:8080");

            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            Client?.Dispose();
            Factory?.Dispose();

            await Task.CompletedTask;
        }

        /// <summary>
        /// Helper method to register a new test user and obtain JWT token
        /// </summary>
        protected async Task<string> RegisterAndLogin(
            string username = "testuser",
            string password = "Test@123")
        {
            // REGISTER

            var registerModel = new
            {
                username,
                password
            };

            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerModel),
                Encoding.UTF8,
                "application/json");

            await Client.PostAsync(
                "/api/authenticate/register",
                registerContent);

            // LOGIN

            var loginModel = new
            {
                username,
                password
            };

            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginModel),
                Encoding.UTF8,
                "application/json");

            var loginResponse = await Client.PostAsync(
                "/api/authenticate/login",
                loginContent);

            if (loginResponse.IsSuccessStatusCode)
            {
                var responseBody =
                    await loginResponse.Content.ReadAsStringAsync();

                using var jsonDoc =
                    JsonDocument.Parse(responseBody);

                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("token", out var tokenElement))
                {
                    return tokenElement.GetString()
                           ?? string.Empty;
                }
            }

            throw new Exception(
                "Failed to obtain authentication token");
        }

        /// <summary>
        /// Sets the Authorization header with Bearer token
        /// </summary>
        protected void SetAuthorizationToken(string token)
        {
            AuthToken = token;

            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);
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