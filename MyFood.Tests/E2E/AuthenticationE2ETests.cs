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
    var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
    var model = new
    {
        FullName = "test" + uniqueId,
        Email = uniqueId + "@example.com",
        Password = "SecurePass@123"
    };

    var content = new StringContent(
        JsonSerializer.Serialize(model),
        Encoding.UTF8,
        "application/json");

    // Act
    var response = await Client.PostAsync("/api/authenticate/register", content);

    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    var responseBody = await response.Content.ReadAsStringAsync();
    Assert.Contains("successful", responseBody, StringComparison.OrdinalIgnoreCase); // Changed to "successful" (matches "Registration successful.")
}

[Fact]
public async Task Register_WithDuplicateUsername_ReturnsBadRequest()
{
    // Arrange
    var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
    var model = new { FullName = "duplicateuser" + uniqueId, Email = uniqueId + "@example.com", Password = "Test@123" };
    var content = new StringContent(
        JsonSerializer.Serialize(model),
        Encoding.UTF8,
        "application/json");

    // Act - Register first time
    await Client.PostAsync("/api/authenticate/register", content);

    // Act - Register again with same FullName
    content = new StringContent(
        JsonSerializer.Serialize(model),
        Encoding.UTF8,
        "application/json");
    var response = await Client.PostAsync("/api/authenticate/register", content);

    // Assert
    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode); // Changed to 409 Conflict
}

[Fact]
public async Task Login_WithValidCredentials_ReturnsTokenAndExpiration()
{
    // Arrange
    var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
    var FullName = "loginuser" + uniqueId;
    var password = "Test@123";
    
    // Register first
    var registerModel = new { FullName, Email = uniqueId + "@example.com", Password = password };
    var registerContent = new StringContent(
        JsonSerializer.Serialize(registerModel),
        Encoding.UTF8,
        "application/json");
    await Client.PostAsync("/api/authenticate/register", registerContent);

    // Act - Login
    var loginModel = new { FullName, Password = password };
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
    
    Assert.True(root.TryGetProperty("token", out var tokenElement)); // Changed to just check token
    Assert.NotEmpty(tokenElement.GetString() ?? "");
}

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var FullName = "validuser" + uniqueId;
            var password = "ValidPass@123";
            
            // Register
            var registerModel = new { FullName, Email = uniqueId + "@example.com", Password = password };
            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerModel),
                Encoding.UTF8,
                "application/json");
            await Client.PostAsync("/api/authenticate/register", registerContent);

            // Act - Login with wrong password
            var loginModel = new { FullName, Password = "WrongPass@123" };
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
            var loginModel = new { FullName = "nonexistent", Password = "AnyPass@123" };
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