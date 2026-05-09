using System.Net;
using System.Text;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        private async Task<int> EnsureFoodIdAsync()
        {
            var token = await RegisterAndLogin($"ingredientuser{Guid.NewGuid():N}".Substring(0, 20), "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/foods?page=1&pageCount=1");
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(responseBody);
                var root = jsonDoc.RootElement;
                if (root.TryGetProperty("value", out var items) && items.GetArrayLength() > 0)
                {
                    return items[0].GetProperty("id").GetInt32();
                }
            }

            var createFood = new
            {
                name = $"IngredientFood{Guid.NewGuid():N}".Substring(0, 18),
                type = "Test",
                calories = 200,
                created = DateTime.UtcNow
            };
            var content = new StringContent(
                JsonSerializer.Serialize(createFood),
                Encoding.UTF8,
                "application/json");

            var createResponse = await Client.PostAsync("/api/v1/foods", content);
            var createBody = await createResponse.Content.ReadAsStringAsync();
            using var createdDoc = JsonDocument.Parse(createBody);
            return createdDoc.RootElement.GetProperty("id").GetInt32();
        }

        [Fact]
        public async Task GetAllIngredients_ReturnsOk()
        {
            var response = await Client.GetAsync("/api/v1/ingredients");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_ThenGetById_ReturnsOk()
        {
            var foodId = await EnsureFoodIdAsync();
            var createIngredient = new
            {
                name = $"Salt{Guid.NewGuid():N}".Substring(0, 12),
                foodId = foodId
            };

            var content = new StringContent(
                JsonSerializer.Serialize(createIngredient),
                Encoding.UTF8,
                "application/json");

            var response = await Client.PostAsync("/api/v1/ingredients", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var id = jsonDoc.RootElement.GetProperty("id").GetInt32();

            var getResponse = await Client.GetAsync($"/api/v1/ingredients/{id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateIngredient_ReturnsNoContent()
        {
            var foodId = await EnsureFoodIdAsync();
            var createIngredient = new
            {
                name = $"Pepper{Guid.NewGuid():N}".Substring(0, 14),
                foodId = foodId
            };

            var content = new StringContent(
                JsonSerializer.Serialize(createIngredient),
                Encoding.UTF8,
                "application/json");
            var createResponse = await Client.PostAsync("/api/v1/ingredients", content);
            var createBody = await createResponse.Content.ReadAsStringAsync();
            using var createDoc = JsonDocument.Parse(createBody);
            var id = createDoc.RootElement.GetProperty("id").GetInt32();

            var updateIngredient = new
            {
                name = "PepperUpdated",
                foodId = foodId
            };

            var updateContent = new StringContent(
                JsonSerializer.Serialize(updateIngredient),
                Encoding.UTF8,
                "application/json");

            var updateResponse = await Client.PutAsync($"/api/v1/ingredients/{id}", updateContent);
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            var getResponse = await Client.GetAsync($"/api/v1/ingredients/{id}");
            var getBody = await getResponse.Content.ReadAsStringAsync();
            using var getDoc = JsonDocument.Parse(getBody);
            var name = getDoc.RootElement.GetProperty("name").GetString();
            Assert.Equal("PepperUpdated", name);
        }

        [Fact]
        public async Task DeleteIngredient_ReturnsNoContent()
        {
            var foodId = await EnsureFoodIdAsync();
            var createIngredient = new
            {
                name = $"Delete{Guid.NewGuid():N}".Substring(0, 12),
                foodId = foodId
            };

            var content = new StringContent(
                JsonSerializer.Serialize(createIngredient),
                Encoding.UTF8,
                "application/json");
            var createResponse = await Client.PostAsync("/api/v1/ingredients", content);
            var createBody = await createResponse.Content.ReadAsStringAsync();
            using var createDoc = JsonDocument.Parse(createBody);
            var id = createDoc.RootElement.GetProperty("id").GetInt32();

            var deleteResponse = await Client.DeleteAsync($"/api/v1/ingredients/{id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await Client.GetAsync($"/api/v1/ingredients/{id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task SearchIngredients_IsCaseInsensitive()
        {
            var foodId = await EnsureFoodIdAsync();
            var createIngredient = new
            {
                name = "CasePepper",
                foodId = foodId
            };

            var content = new StringContent(
                JsonSerializer.Serialize(createIngredient),
                Encoding.UTF8,
                "application/json");
            await Client.PostAsync("/api/v1/ingredients", content);

            var response = await Client.GetAsync("/api/v1/ingredients/search?name=casepepper");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            var items = doc.RootElement.EnumerateArray();
            Assert.Contains(items, item => item.GetProperty("name").GetString()?.Contains("CasePepper", StringComparison.OrdinalIgnoreCase) == true);
        }
    }
}
