using System.Net;
using System.Text;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class AuthenticationE2ETests : ApiE2ETestBase
    {
        [Fact]
        public async Task Register_WithValidCredentials_ReturnsOk()
        {
            var username = $"test_{Guid.NewGuid():N}";

            var model = new
            {
                username,
                password = "SecurePass@123"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");

            var response = await Client.PostAsync("/api/authenticate/register", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            Assert.True(root.TryGetProperty("token", out var tokenElement));
            Assert.True(root.TryGetProperty("expiration", out _));
            Assert.True(root.TryGetProperty("username", out var usernameElement));

            Assert.NotEmpty(tokenElement.GetString() ?? string.Empty);
            Assert.Equal(username, usernameElement.GetString());
        }

        [Fact]
        public async Task Register_WithDuplicateUsername_ReturnsBadRequest()
        {
            var username = $"duplicateuser_{Guid.NewGuid():N}";
            var password = "Test@123";

            var model = new { username, password };

            var firstContent = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");

            var firstResponse = await Client.PostAsync("/api/authenticate/register", firstContent);

            var secondContent = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");

            var secondResponse = await Client.PostAsync("/api/authenticate/register", secondContent);

            Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsTokenAndExpiration()
        {
            var username = $"loginuser_{Guid.NewGuid():N}";
            var password = "Test@123";

            var registerModel = new { username, password };

            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerModel),
                Encoding.UTF8,
                "application/json");

            await Client.PostAsync("/api/authenticate/register", registerContent);

            var loginModel = new { username, password };

            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginModel),
                Encoding.UTF8,
                "application/json");

            var response = await Client.PostAsync("/api/authenticate/login", loginContent);
            var responseBody = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            Assert.True(root.TryGetProperty("token", out var tokenElement));
            Assert.True(root.TryGetProperty("expiration", out _));
            Assert.True(root.TryGetProperty("username", out var usernameElement));

            Assert.NotEmpty(tokenElement.GetString() ?? string.Empty);
            Assert.Equal(username, usernameElement.GetString());
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            var username = $"validuser_{Guid.NewGuid():N}";
            var password = "ValidPass@123";

            var registerModel = new { username, password };

            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerModel),
                Encoding.UTF8,
                "application/json");

            await Client.PostAsync("/api/authenticate/register", registerContent);

            var loginModel = new
            {
                username,
                password = "WrongPass@123"
            };

            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginModel),
                Encoding.UTF8,
                "application/json");

            var response = await Client.PostAsync("/api/authenticate/login", loginContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithNonexistentUser_ReturnsUnauthorized()
        {
            var loginModel = new
            {
                username = $"nonexistent_{Guid.NewGuid():N}",
                password = "AnyPass@123"
            };

            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginModel),
                Encoding.UTF8,
                "application/json");

            var response = await Client.PostAsync("/api/authenticate/login", loginContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}