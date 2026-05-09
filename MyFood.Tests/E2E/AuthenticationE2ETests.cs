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
            // Arrange
            var un = "test" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var model = new
            {
                username = un,
                email = $"{un}@test.com",
                password = "SecurePass@123"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await Client.PostAsync("/api/authenticate/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Contains("token", responseBody, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Register_WithDuplicateUsername_ReturnsBadRequest()
        {
            // Arrange
            var username = "duplicateuser";
            var password = "Test@123";

            var model = new { username, email = $"{username}@test.com", password };
            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");

            // Act - Register first time
            await Client.PostAsync("/api/authenticate/register", content);

            // Act - Register again with same username
            content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/authenticate/register", content);

            // Assert — controller returns Conflict (409) for duplicate usernames
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsTokenAndExpiration()
        {
            // Arrange
            var username = "loginuser";
            var password = "Test@123";

            // Register first
            var registerModel = new { username, email = $"{username}@test.com", password };
            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerModel),
                Encoding.UTF8,
                "application/json");
            await Client.PostAsync("/api/authenticate/register", registerContent);

            // Act - Login
            var loginModel = new { username, password };
            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginModel),
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/authenticate/login", loginContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            
            Assert.True(root.TryGetProperty("token", out var tokenElement));
            Assert.True(root.TryGetProperty("expiration", out var expirationElement));
            Assert.NotEmpty(tokenElement.GetString() ?? "");
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var username = "validuser";
            var password = "ValidPass@123";

            // Register
            var registerModel = new { username, email = $"{username}@test.com", password };
            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerModel),
                Encoding.UTF8,
                "application/json");
            await Client.PostAsync("/api/authenticate/register", registerContent);

            // Act - Login with wrong password
            var loginModel = new { username, password = "WrongPass@123" };
            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginModel),
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/authenticate/login", loginContent);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithNonexistentUser_ReturnsUnauthorized()
        {
            // Arrange
            var loginModel = new { username = "nonexistent", password = "AnyPass@123" };
            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginModel),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await Client.PostAsync("/api/authenticate/login", loginContent);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
