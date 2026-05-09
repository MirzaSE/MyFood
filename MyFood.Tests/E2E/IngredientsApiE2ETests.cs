using System.Net;
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
            // Act
            var response = await Client.GetAsync("/api/v1/ingredients");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllIngredients_WithAuthentication_ReturnsOk()
        {
            // Arrange
            var token = await RegisterAndLogin("ingreduser1", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v1/ingredients?page=1&pageCount=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"), "Should contain X-Pagination header");

            var paginationHeaders = response.Headers.GetValues("X-Pagination");
            var paginationJson = paginationHeaders.First();
            using var jsonDoc = JsonDocument.Parse(paginationJson);
            var pagination = jsonDoc.RootElement;
            Assert.True(pagination.TryGetProperty("currentPage", out _));
            Assert.True(pagination.TryGetProperty("pageSize", out _));
            Assert.True(pagination.TryGetProperty("totalCount", out _));
        }

        [Fact]
        public async Task GetSingleIngredient_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var token = await RegisterAndLogin("ingreduser2", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v1/ingredients/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_WithValidData_ReturnsCreated()
        {
            // Arrange
            var token = await RegisterAndLogin("ingreduser3", "Test@123");
            SetAuthorizationToken(token);

            var ingredientPayload = new
            {
                name = "TestSalt_" + Guid.NewGuid().ToString("N")[..8],
                unit = "g",
                caloriesPerUnit = 0.1,
                protein = 0.0,
                carbs = 0.0,
                fat = 0.0
            };

            var content = new StringContent(
                JsonSerializer.Serialize(ingredientPayload),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await Client.PostAsync("/api/v1/ingredients", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            Assert.True(root.TryGetProperty("id", out var id));
            Assert.True(id.GetInt32() > 0);
            Assert.True(root.TryGetProperty("unit", out var unit));
            Assert.Equal("g", unit.GetString());
        }

        [Fact]
        public async Task DeleteIngredient_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var token = await RegisterAndLogin("ingreduser4", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.DeleteAsync("/api/v1/ingredients/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchIngredients_WithEmptyName_ReturnsBadRequest()
        {
            // Arrange
            var token = await RegisterAndLogin("ingreduser5", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v1/ingredients/search?name=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_ThenGetById_ReturnsCorrectData()
        {
            // Arrange
            var token = await RegisterAndLogin("ingreduser6", "Test@123");
            SetAuthorizationToken(token);

            var uniqueName = "Pepper_" + Guid.NewGuid().ToString("N")[..8];
            var ingredientPayload = new
            {
                name = uniqueName,
                unit = "tsp",
                caloriesPerUnit = 2.5,
                protein = 0.1,
                carbs = 0.4,
                fat = 0.05
            };

            var createContent = new StringContent(
                JsonSerializer.Serialize(ingredientPayload),
                Encoding.UTF8,
                "application/json");

            var createResponse = await Client.PostAsync("/api/v1/ingredients", createContent);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createBody = await createResponse.Content.ReadAsStringAsync();
            using var createDoc = JsonDocument.Parse(createBody);
            var createdId = createDoc.RootElement.GetProperty("id").GetInt32();

            // Act
            var getResponse = await Client.GetAsync($"/api/v1/ingredients/{createdId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var getBody = await getResponse.Content.ReadAsStringAsync();
            using var getDoc = JsonDocument.Parse(getBody);
            var ingredient = getDoc.RootElement;
            Assert.Equal(createdId, ingredient.GetProperty("id").GetInt32());
            Assert.Equal(uniqueName, ingredient.GetProperty("name").GetString());
        }
    }
}
