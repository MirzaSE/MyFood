using System.Net;
using System.Text;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        private async Task<string> SetupAuthAsync(string username = "ingredientuser")
        {
            var token = await RegisterAndLogin(username, "Test@123");
            SetAuthorizationToken(token);
            return token;
        }

        private async Task<int> CreateTestIngredientAsync()
        {
            var ingredientData = new
            {
                name = $"Ingredient_{Guid.NewGuid():N}",
                unit = "g",
                caloriesPerUnit = 50.0,
                protein = 5.0,
                carbs = 10.0,
                fat = 2.0
            };
            var content = new StringContent(JsonSerializer.Serialize(ingredientData), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("/api/v1/ingredients", content);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.GetProperty("id").GetInt32();
        }

        [Fact]
        public async Task GetAllIngredients_WithoutAuth_ReturnsUnauthorized()
        {
            ClearAuthorizationToken();
            var response = await Client.GetAsync("/api/v1/ingredients");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllIngredients_WithAuth_ReturnsOk()
        {
            await SetupAuthAsync("ingruser1");

            var response = await Client.GetAsync("/api/v1/ingredients?page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"));

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.True(doc.RootElement.ValueKind == JsonValueKind.Array);
        }

        [Fact]
        public async Task CreateIngredient_WithValidData_ReturnsCreated()
        {
            await SetupAuthAsync("ingruser2");

            var ingredientData = new
            {
                name = "Salt",
                unit = "tsp",
                caloriesPerUnit = 0.0,
                protein = 0.0,
                carbs = 0.0,
                fat = 0.0
            };
            var content = new StringContent(JsonSerializer.Serialize(ingredientData), Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("/api/v1/ingredients", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.Equal("Salt", doc.RootElement.GetProperty("name").GetString());
            Assert.Equal("tsp", doc.RootElement.GetProperty("unit").GetString());
            Assert.True(doc.RootElement.GetProperty("id").GetInt32() > 0);
        }

        [Fact]
        public async Task GetIngredient_ById_ReturnsOk()
        {
            await SetupAuthAsync("ingruser3");
            var ingredientId = await CreateTestIngredientAsync();

            var response = await Client.GetAsync($"/api/v1/ingredients/{ingredientId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.Equal(ingredientId, doc.RootElement.GetProperty("id").GetInt32());
        }

        [Fact]
        public async Task GetIngredient_NotFound_Returns404()
        {
            await SetupAuthAsync("ingruser4");

            var response = await Client.GetAsync("/api/v1/ingredients/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateIngredient_Succeeds()
        {
            await SetupAuthAsync("ingruser5");
            var ingredientId = await CreateTestIngredientAsync();

            var updateData = new { name = "Updated Ingredient", unit = "kg", caloriesPerUnit = 200.0 };
            var content = new StringContent(JsonSerializer.Serialize(updateData), Encoding.UTF8, "application/json");

            var response = await Client.PutAsync($"/api/v1/ingredients/{ingredientId}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.Equal("Updated Ingredient", doc.RootElement.GetProperty("name").GetString());
            Assert.Equal("kg", doc.RootElement.GetProperty("unit").GetString());
        }

        [Fact]
        public async Task DeleteIngredient_Succeeds()
        {
            await SetupAuthAsync("ingruser6");
            var ingredientId = await CreateTestIngredientAsync();

            var response = await Client.DeleteAsync($"/api/v1/ingredients/{ingredientId}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify it's gone
            var getResponse = await Client.GetAsync($"/api/v1/ingredients/{ingredientId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteIngredient_NotFound_Returns404()
        {
            await SetupAuthAsync("ingruser7");

            var response = await Client.DeleteAsync("/api/v1/ingredients/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchIngredients_ReturnsResults()
        {
            await SetupAuthAsync("ingruser8");

            // Create ingredient with a unique searchable name
            var uniqueName = $"FindMe_{Guid.NewGuid():N}";
            var ingredientData = new
            {
                name = uniqueName,
                unit = "g",
                caloriesPerUnit = 10.0,
                protein = 1.0,
                carbs = 2.0,
                fat = 0.5
            };
            var content = new StringContent(JsonSerializer.Serialize(ingredientData), Encoding.UTF8, "application/json");
            await Client.PostAsync("/api/v1/ingredients", content);

            var response = await Client.GetAsync($"/api/v1/ingredients/search?name={uniqueName}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.True(doc.RootElement.GetArrayLength() >= 1);
        }
    }
}
