 using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using MyFood.Api;
using System.Net.Http.Headers;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

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
        protected async Task<string> RegisterAndLogin(string FullName = "testuser", string password = "Test@123", string email = "")
{
    if (string.IsNullOrWhiteSpace(email))
    {
        email = $"{FullName}_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
    }

    var registerModel = new { FullName, Email = email, Password = password };
    var registerContent = new StringContent(
        JsonSerializer.Serialize(registerModel),
        Encoding.UTF8,
        "application/json");

    var registerResponse = await Client.PostAsync("/api/authenticate/register", registerContent);
    var registerBody = await registerResponse.Content.ReadAsStringAsync();

    if (!registerResponse.IsSuccessStatusCode)
    {
        if (registerResponse.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            // User already exists — attempt login only
        }
        else
        {
            throw new Exception($"Register failed: {registerResponse.StatusCode}. Body: {registerBody}");
        }
    }

    // Attempt login
    var loginModel = new { FullName, Password = password };
    var loginContent = new StringContent(
        JsonSerializer.Serialize(loginModel),
        Encoding.UTF8,
        "application/json");

    var loginResponse = await Client.PostAsync("/api/authenticate/login", loginContent);
    var loginBody = await loginResponse.Content.ReadAsStringAsync();

    if (!loginResponse.IsSuccessStatusCode)
    {
        throw new Exception($"Login failed: {loginResponse.StatusCode}. Body: {loginBody}");
    }

    using var jsonDoc = JsonDocument.Parse(loginBody);
    var root = jsonDoc.RootElement;

    if (root.TryGetProperty("token", out var tokenElement) && !string.IsNullOrEmpty(tokenElement.GetString()))
    {
        return tokenElement.GetString() ?? string.Empty;
    }

    throw new Exception($"Login response did not contain valid token. Body: {loginBody}");
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
