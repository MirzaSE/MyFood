using System.Net;
using System.Text;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        [Fact]
        public async Task GetAllIngredients_ReturnsOkWithPagination()
        {
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
        public async Task GetSingleIngredient_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await Client.GetAsync("/api/v1/ingredients/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetSingleIngredient_WithNegativeId_ReturnsInternalServerError()
        {
            // Act
            var response = await Client.GetAsync("/api/v1/ingredients/-1");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task UpdateIngredient_WithMissingId_ReturnsNotFound()
        {
            // Arrange
            var updateModel = new
            {
                name = "Updated Name",
                quantity = 8,
                foodEntityId = 1
            };

            var updateContent = new StringContent(
                JsonSerializer.Serialize(updateModel),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await Client.PutAsync("/api/v1/ingredients/99999", updateContent);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteIngredient_WithMissingId_ReturnsNotFound()
        {
            // Act
            var response = await Client.DeleteAsync("/api/v1/ingredients/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private async Task<int> GetExistingFoodIdAsync()
        {
            var token = await RegisterAndLogin($"ingredientfood{Guid.NewGuid():N}".Substring(0, 18), "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/foods?page=1&pageCount=1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var foods = jsonDoc.RootElement.GetProperty("value");
            Assert.True(foods.GetArrayLength() > 0, "Expected at least one food item in test environment.");

            var foodId = foods[0].GetProperty("id").GetInt32();

            ClearAuthorizationToken();
            return foodId;
        }

        [Fact]
        public async Task GetAllIngredients_WithLargePageCount_ReturnsCappedPageCount()
        {
            // Act
            var response = await Client.GetAsync("/api/v1/ingredients?page=1&pageCount=1000");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.TryGetValues("X-Pagination", out var paginationHeaders));

            var paginationJson = paginationHeaders?.First();
            Assert.NotNull(paginationJson);
            using var jsonDoc = JsonDocument.Parse(paginationJson);
            var pagination = jsonDoc.RootElement;

            var pageSize = pagination.GetProperty("pageSize").GetInt32();
            Assert.True(pageSize <= 50, $"Expected page size <= 50, but got {pageSize}");
        }
    }
}
