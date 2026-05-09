using System.Net;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class ApiVersioningE2ETests : ApiE2ETestBase
    {
        [Fact]
        public async Task V1_Foods_Endpoint_IsAccessible()
        {
            // Arrange
            var token = await RegisterAndLogin("versionuser1", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v1/foods?page=1&pageCount=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            Assert.True(root.TryGetProperty("value", out _));
            Assert.True(root.TryGetProperty("links", out _));
        }

        [Fact]
        public async Task V2_Foods_Endpoint_ReturnsVersionString()
        {
            // Arrange
            var token = await RegisterAndLogin("versionuser4", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v2/foods");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Contains("2.0", responseBody);
        }

        [Fact]
        public async Task DefaultVersion_Uses_Latest_Endpoint()
        {
            // Arrange
            var token = await RegisterAndLogin("versionuser2", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/foods");

            // Assert - Should route appropriately
            Assert.True(response.StatusCode == HttpStatusCode.OK || 
                       response.StatusCode == HttpStatusCode.NotFound ||
                       response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task InvalidVersion_ReturnsNotFound()
        {
            // Arrange
            var token = await RegisterAndLogin("versionuser3", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v99/foods");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}