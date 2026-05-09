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
        }

        public Task DisposeAsync()
        {
            Client?.Dispose();
            Factory?.Dispose();
            return Task.CompletedTask;
        }

        protected async Task<string> RegisterAndLogin(string username, string password)
        {
            var email = $"{username}@test.com";
            
            // Register using correct endpoint
            var registerDto = new { Username = username, Email = email, Password = password };
            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerDto),
                Encoding.UTF8,
                "application/json");

            try
            {
                await Client.PostAsync("/api/Auth/register", registerContent);
            }
            catch
            {
                // User might already exist
            }

            // Login using correct endpoint
            var loginDto = new { Username = username, Password = password };
            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginDto),
                Encoding.UTF8,
                "application/json");

            var loginResponse = await Client.PostAsync("/api/Auth/login", loginContent);
            
            if (loginResponse.IsSuccessStatusCode)
            {
                var responseBody = await loginResponse.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(responseBody);
                var root = jsonDoc.RootElement;
                
                // Your AuthResponseDto might have token directly or in a property
                if (root.TryGetProperty("token", out var tokenElement))
                {
                    return tokenElement.GetString() ?? string.Empty;
                }
                // Alternative: token might be the root itself
                else if (root.ValueKind == JsonValueKind.String)
                {
                    return root.GetString() ?? string.Empty;
                }
            }

            return string.Empty;
        }

        protected void SetAuthorizationToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                AuthToken = token;
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        protected void ClearAuthorizationToken()
        {
            AuthToken = null;
            Client.DefaultRequestHeaders.Authorization = null;
        }
    }
}