using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        [Fact]
        public async Task GetAllIngredients_WithoutAuthentication_ReturnsUnauthorized()
        {
            var response = await Client.GetAsync("/api/v1/ingredients");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllIngredients_WithAuthentication_ReturnsOk()
        {
            var token = await RegisterAndLogin("ingredientuser1", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_WithValidData_ReturnsCreated()
        {
            var token = await RegisterAndLogin("ingredientuser2", "Test@123");
            SetAuthorizationToken(token);

            var ingredient = new { name = "Test Salt", quantity = "10g", foodId = (int?)null };
            var content = new StringContent(
                JsonSerializer.Serialize(ingredient),
                Encoding.UTF8,
                "application/json");

            var response = await Client.PostAsync("/api/v1/ingredients", content);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            Assert.True(root.TryGetProperty("name", out var nameElement));
            Assert.Equal("Test Salt", nameElement.GetString());
        }

        [Fact]
        public async Task GetSingleIngredient_WithInvalidId_ReturnsNotFound()
        {
            var token = await RegisterAndLogin("ingredientuser3", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteIngredient_WithInvalidId_ReturnsNotFound()
        {
            var token = await RegisterAndLogin("ingredientuser4", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.DeleteAsync("/api/v1/ingredients/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateIngredient_WithInvalidId_ReturnsErrorStatus()
        {
            var token = await RegisterAndLogin("ingredientuser5", "Test@123");
            SetAuthorizationToken(token);

            var ingredient = new { name = "Updated", quantity = "5g", foodId = (int?)null };
            var content = new StringContent(
                JsonSerializer.Serialize(ingredient),
                Encoding.UTF8,
                "application/json");

            var response = await Client.PutAsync("/api/v1/ingredients/99999", content);
            Assert.False(response.IsSuccessStatusCode);
            Assert.True(
                response.StatusCode == HttpStatusCode.NotFound ||
                response.StatusCode == HttpStatusCode.BadRequest,
                $"Expected NotFound or BadRequest, got {response.StatusCode}");
        }

        [Fact]
        public async Task GetAllIngredients_WithInvalidToken_ReturnsUnauthorized()
        {
            SetAuthorizationToken("invalid.jwt.token");

            var response = await Client.GetAsync("/api/v1/ingredients");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
