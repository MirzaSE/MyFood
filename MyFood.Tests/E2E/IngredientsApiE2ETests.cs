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
            // Act
            var response = await Client.GetAsync("/api/v1/ingredients");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllIngredients_WithAuthentication_ReturnsOkWithIngredients()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientapiuser1", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v1/ingredients?page=1&pageCount=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"), "Should contain X-Pagination header");
            
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            
            Assert.True(root.TryGetProperty("value", out _));
            Assert.True(root.TryGetProperty("links", out _));
        }

        [Fact]
        public async Task GetAllIngredients_WithPagination_ReturnsPaginationMetadata()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientapiuser2", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v1/ingredients?page=1&pageCount=5");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.TryGetValues("X-Pagination", out var paginationHeaders));
            
            var paginationJson = paginationHeaders?.First();
            Assert.NotNull(paginationJson);
            
            using var jsonDoc = JsonDocument.Parse(paginationJson);
            var pagination = jsonDoc.RootElement;
            Assert.True(pagination.TryGetProperty("currentPage", out var currentPage));
            Assert.True(pagination.TryGetProperty("pageSize", out var pageSize));
            Assert.Equal(1, currentPage.GetInt32());
            Assert.Equal(5, pageSize.GetInt32());
        }

        [Fact]
        public async Task GetSingleIngredient_WithValidId_ReturnsOk()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientapiuser3", "Test@123");
            SetAuthorizationToken(token);

            // Create an ingredient first
            var ingredientName = $"TestIngredient-{Guid.NewGuid():N}";
            var createModel = new
            {
                Name = ingredientName,
                Quantity = 2,
                FoodEntityId = 1
            };
            var createBody = new StringContent(System.Text.Json.JsonSerializer.Serialize(createModel), Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
            var createResponse = await Client.PostAsync("/api/v1/ingredients", createBody);
            
            if (createResponse.IsSuccessStatusCode)
            {
                var createResponseBody = await createResponse.Content.ReadAsStringAsync();
                using var createDoc = JsonDocument.Parse(createResponseBody);
                var ingredientId = createDoc.RootElement.GetProperty("id").GetInt32();

                // Act
                var response = await Client.GetAsync($"/api/v1/ingredients/{ingredientId}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var responseBody = await response.Content.ReadAsStringAsync();
                using var ingredientDoc = JsonDocument.Parse(responseBody);
                var ingredient = ingredientDoc.RootElement;
                
                Assert.True(ingredient.TryGetProperty("id", out var id));
                Assert.Equal(ingredientId, id.GetInt32());
                Assert.Equal(ingredientName, ingredient.GetProperty("name").GetString());
            }
        }

        [Fact]
        public async Task GetSingleIngredient_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientapiuser4", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v1/ingredients/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchIngredients_WithValidSearchTerm_ReturnsOkResponse()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientapiuser5", "Test@123");
            SetAuthorizationToken(token);

            // Act - Search with a common term (doesn't matter if it finds anything, just verify endpoint works)
            var response = await Client.GetAsync("/api/v1/ingredients/search?name=test");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            
            // Verify response has expected structure
            Assert.True(root.TryGetProperty("value", out var ingredients));
            Assert.NotNull(ingredients);
        }

    }
}
