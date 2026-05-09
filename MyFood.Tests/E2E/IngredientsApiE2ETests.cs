using System.Net;
using System.Text;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        [Fact]
        public async Task GetAllIngredients_ReturnsOkWithPaginationHeader()
        {
            var response = await Client.GetAsync("/api/v1/ingredient?page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"), "Should contain X-Pagination header");

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.True(doc.RootElement.ValueKind == JsonValueKind.Array || doc.RootElement.ValueKind == JsonValueKind.Object);
        }

        [Fact]
        public async Task GetSingleIngredient_WithInvalidId_ReturnsNotFound()
        {
            var response = await Client.GetAsync("/api/v1/ingredient/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetSingleIngredient_WithNegativeId_ReturnsError()
        {
            var response = await Client.GetAsync("/api/v1/ingredient/-1");

            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.InternalServerError,
                $"Expected BadRequest or InternalServerError, got {response.StatusCode}");
        }

        [Fact]
        public async Task AddIngredient_WithValidData_ReturnsOk()
        {
            var payload = new { name = "TestIngredient", unit = "g", caloriesPerUnit = 100, protein = 5, carbs = 10, fat = 2 };
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await Client.PostAsync("/api/v1/ingredient", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteIngredient_WithNonExistentId_ReturnsNotFound()
        {
            var response = await Client.DeleteAsync("/api/v1/ingredient/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateIngredient_WithNonExistentId_ReturnsNotFound()
        {
            var payload = new { name = "Updated" };
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await Client.PutAsync("/api/v1/ingredient/99999", content);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllIngredients_WithPagination_ReturnsOk()
        {
            var response = await Client.GetAsync("/api/v1/ingredient?page=1&pageCount=5");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}