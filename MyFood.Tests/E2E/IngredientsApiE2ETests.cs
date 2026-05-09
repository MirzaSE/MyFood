using System.Net;
using System.Text;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        private StringContent JsonContent(object obj) =>
            new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

        [Fact]
        public async Task GetAllIngredients_ReturnsOkWithPaginationHeader()
        {
            var response = await Client.GetAsync("/api/v1/ingredient?page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"), "Should contain X-Pagination header");

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.True(doc.RootElement.TryGetProperty("value", out _));
        }

        [Fact]
        public async Task GetSingleIngredient_WithInvalidId_ReturnsNotFound()
        {
            var response = await Client.GetAsync("/api/v1/ingredient/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_WithValidBody_ReturnsCreated()
        {
            var body = JsonContent(new { name = "TestIngredient_E2E", quantity = "100g" });

            var response = await Client.PostAsync("/api/v1/ingredient", body);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);
            Assert.True(doc.RootElement.TryGetProperty("name", out var name));
            Assert.Equal("TestIngredient_E2E", name.GetString());
        }

        [Fact]
        public async Task UpdateIngredient_WithValidId_ReturnsOk()
        {
            // Create first
            var created = await Client.PostAsync("/api/v1/ingredient",
                JsonContent(new { name = "UpdateTarget_E2E", quantity = "50g" }));
            Assert.Equal(HttpStatusCode.Created, created.StatusCode);

            var createdBody = await created.Content.ReadAsStringAsync();
            using var createdDoc = JsonDocument.Parse(createdBody);
            var id = createdDoc.RootElement.GetProperty("id").GetInt32();

            // Update
            var response = await Client.PutAsync($"/api/v1/ingredient/{id}",
                JsonContent(new { name = "UpdatedIngredient_E2E", quantity = "75g" }));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteIngredient_WithValidId_ReturnsNoContent()
        {
            // Create first
            var created = await Client.PostAsync("/api/v1/ingredient",
                JsonContent(new { name = "DeleteTarget_E2E", quantity = "10g" }));
            Assert.Equal(HttpStatusCode.Created, created.StatusCode);

            var createdBody = await created.Content.ReadAsStringAsync();
            using var createdDoc = JsonDocument.Parse(createdBody);
            var id = createdDoc.RootElement.GetProperty("id").GetInt32();

            // Delete
            var response = await Client.DeleteAsync($"/api/v1/ingredient/{id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_WithNullBody_ReturnsBadRequest()
        {
            var response = await Client.PostAsync("/api/v1/ingredient",
                new StringContent("null", Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
