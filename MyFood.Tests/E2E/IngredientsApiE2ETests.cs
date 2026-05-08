using System.Net;
using System.Text;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        private async Task<int> GetExistingFoodId()
        {
            var token = await RegisterAndLogin($"ingredient-user-{Guid.NewGuid():N}".Substring(0, 20), "Test@123");
            SetAuthorizationToken(token);

            var foodsResponse = await Client.GetAsync("/api/v1/foods?page=1&pageCount=10");
            foodsResponse.EnsureSuccessStatusCode();
            var body = await foodsResponse.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(body);
            var value = json.RootElement.GetProperty("value");
            if (value.GetArrayLength() == 0)
            {
                throw new InvalidOperationException("Seed data did not return foods.");
            }

            return value[0].GetProperty("id").GetInt32();
        }

        [Fact]
        public async Task GetAllIngredients_ReturnsOkWithEnvelope()
        {
            var response = await Client.GetAsync("/api/v1/ingredients?page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"));

            var body = await response.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(body);
            Assert.True(json.RootElement.TryGetProperty("value", out _));
            Assert.True(json.RootElement.TryGetProperty("links", out _));
        }

        [Fact]
        public async Task GetSingleIngredient_WithInvalidId_ReturnsNotFound()
        {
            var response = await Client.GetAsync("/api/v1/ingredients/999999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_WithInvalidFoodId_ReturnsBadRequest()
        {
            var payload = new
            {
                name = "E2E-Bad-Food",
                quantity = 3,
                foodEntityId = 999999
            };

            var response = await Client.PostAsync(
                "/api/v1/ingredients",
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_UpdateIngredient_DeleteIngredient_RoundTrip()
        {
            var foodId = await GetExistingFoodId();

            var createPayload = new
            {
                name = "E2E-Salt",
                quantity = 1,
                foodEntityId = foodId
            };

            var createResponse = await Client.PostAsync(
                "/api/v1/ingredients",
                new StringContent(JsonSerializer.Serialize(createPayload), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdBody = await createResponse.Content.ReadAsStringAsync();
            using var createdJson = JsonDocument.Parse(createdBody);
            var ingredientId = createdJson.RootElement.GetProperty("id").GetInt32();

            var updatePayload = new
            {
                name = "E2E-Salt-Updated",
                quantity = 2,
                foodEntityId = foodId
            };

            var updateResponse = await Client.PutAsync(
                $"/api/v1/ingredients/{ingredientId}",
                new StringContent(JsonSerializer.Serialize(updatePayload), Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            var getResponse = await Client.GetAsync($"/api/v1/ingredients/{ingredientId}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getBody = await getResponse.Content.ReadAsStringAsync();
            using var getJson = JsonDocument.Parse(getBody);
            Assert.Equal("E2E-Salt-Updated", getJson.RootElement.GetProperty("name").GetString());

            var deleteResponse = await Client.DeleteAsync($"/api/v1/ingredients/{ingredientId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task GetAllIngredients_WithSearchQuery_ReturnsOk()
        {
            var response = await Client.GetAsync("/api/v1/ingredients?page=1&pageCount=10&query=salt");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
