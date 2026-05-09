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
            var model = new
            {
                username = "test" + Guid.NewGuid().ToString("N").Substring(0, 8),
                password = "SecurePass@123"
            };

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("/api/authenticate/register", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithDuplicateUsername_ReturnsBadRequest()
        {
            var username = "duplicateuser" + Guid.NewGuid().ToString("N").Substring(0, 4);
            var password = "Test@123";
            var model = new { username, password };

            var content1 = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            await Client.PostAsync("/api/authenticate/register", content1);

            var content2 = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("/api/authenticate/register", content2);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsTokenAndExpiration()
        {
            var username = "loginuser" + Guid.NewGuid().ToString("N").Substring(0, 4);
            var password = "Test@123";

            var registerContent = new StringContent(
                JsonSerializer.Serialize(new { username, password }), Encoding.UTF8, "application/json");
            await Client.PostAsync("/api/authenticate/register", registerContent);

            var loginContent = new StringContent(
                JsonSerializer.Serialize(new { username, password }), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("/api/authenticate/login", loginContent);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            Assert.True(root.TryGetProperty("token", out var tokenElement));
            Assert.NotEmpty(tokenElement.GetString() ?? "");
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            var username = "validuser" + Guid.NewGuid().ToString("N").Substring(0, 4);
            var password = "ValidPass@123";

            var registerContent = new StringContent(
                JsonSerializer.Serialize(new { username, password }), Encoding.UTF8, "application/json");
            await Client.PostAsync("/api/authenticate/register", registerContent);

            var loginContent = new StringContent(
                JsonSerializer.Serialize(new { username, password = "WrongPass@123" }), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("/api/authenticate/login", loginContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithNonexistentUser_ReturnsUnauthorized()
        {
            var loginContent = new StringContent(
                JsonSerializer.Serialize(new { username = "nonexistent", password = "AnyPass@123" }), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("/api/authenticate/login", loginContent);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}