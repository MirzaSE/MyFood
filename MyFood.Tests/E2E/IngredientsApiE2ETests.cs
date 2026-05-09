using System.Net;
using System.Text;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        [Fact]
        public async Task GetAllIngredients_ReturnsOkWithCollectionAndPagination()
        {
            await CreateIngredientAsync("Flour", 2);
            await CreateIngredientAsync("Eggs", 12);

            var response = await Client.GetAsync("/api/v1/ingredients?page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"));

            using var jsonDoc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Assert.True(jsonDoc.RootElement.TryGetProperty("value", out var value));
            Assert.True(value.GetArrayLength() >= 2);
            Assert.True(jsonDoc.RootElement.TryGetProperty("links", out _));
        }

        [Fact]
        public async Task CreateIngredient_WithValidPayload_ReturnsCreatedIngredient()
        {
            var response = await CreateIngredientAsync("Sugar", 5);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            using var jsonDoc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Assert.Equal("Sugar", jsonDoc.RootElement.GetProperty("name").GetString());
            Assert.Equal(5, jsonDoc.RootElement.GetProperty("quantity").GetInt32());
            Assert.True(jsonDoc.RootElement.GetProperty("id").GetInt32() > 0);
        }

        [Fact]
        public async Task CreateIngredient_WithMissingName_ReturnsBadRequest()
        {
            var payload = new { quantity = 1 };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("/api/v1/ingredients", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetSingleIngredient_WithExistingId_ReturnsIngredient()
        {
            var id = await CreateIngredientAndReadIdAsync("Milk", 1);

            var response = await Client.GetAsync($"/api/v1/ingredients/{id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var jsonDoc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Assert.Equal(id, jsonDoc.RootElement.GetProperty("id").GetInt32());
            Assert.Equal("Milk", jsonDoc.RootElement.GetProperty("name").GetString());
        }

        [Fact]
        public async Task GetSingleIngredient_WithMissingId_ReturnsNotFound()
        {
            var response = await Client.GetAsync("/api/v1/ingredients/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchIngredients_ByName_ReturnsMatches()
        {
            await CreateIngredientAsync("Brown Sugar", 2);
            await CreateIngredientAsync("Olive Oil", 1);

            var response = await Client.GetAsync("/api/v1/ingredients/search?name=sugar&page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var jsonDoc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var value = jsonDoc.RootElement.GetProperty("value");
            Assert.Contains(value.EnumerateArray(), x => x.GetProperty("name").GetString() == "Brown Sugar");
        }

        [Fact]
        public async Task UpdateIngredient_WithValidPayload_ReturnsUpdatedIngredient()
        {
            var id = await CreateIngredientAndReadIdAsync("Salt", 1);
            var payload = new { name = "Sea Salt", quantity = 3 };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await Client.PutAsync($"/api/v1/ingredients/{id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var jsonDoc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Assert.Equal("Sea Salt", jsonDoc.RootElement.GetProperty("name").GetString());
            Assert.Equal(3, jsonDoc.RootElement.GetProperty("quantity").GetInt32());
        }

        [Fact]
        public async Task DeleteIngredient_WithExistingId_ReturnsNoContent()
        {
            var id = await CreateIngredientAndReadIdAsync("Pepper", 1);

            var response = await Client.DeleteAsync($"/api/v1/ingredients/{id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var getResponse = await Client.GetAsync($"/api/v1/ingredients/{id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        private async Task<HttpResponseMessage> CreateIngredientAsync(string name, int quantity)
        {
            var payload = new { name, quantity };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            return await Client.PostAsync("/api/v1/ingredients", content);
        }

        private async Task<int> CreateIngredientAndReadIdAsync(string name, int quantity)
        {
            var response = await CreateIngredientAsync(name, quantity);
            response.EnsureSuccessStatusCode();

            using var jsonDoc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return jsonDoc.RootElement.GetProperty("id").GetInt32();
        }
    }
}
