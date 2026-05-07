using System.Net;
using System.Text;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        [Fact]
        public async Task GetAllIngredients_ReturnsOk()
        {
            var response = await Client.GetAsync("/api/v1/ingredients?page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            await AssertJsonArray(response);
        }

        [Fact]
        public async Task CreateIngredient_WithValidPayload_ReturnsCreatedAndIngredient()
        {
            var name = UniqueName();

            var response = await CreateIngredient(name);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            using var jsonDoc = await ReadJson(response);
            var ingredient = jsonDoc.RootElement;
            Assert.True(ingredient.GetProperty("id").GetInt32() > 0);
            Assert.Equal(name, ingredient.GetProperty("name").GetString());
            Assert.Equal("g", ingredient.GetProperty("unit").GetString());

            await DeleteIngredientIfExists(ingredient.GetProperty("id").GetInt32());
        }

        [Fact]
        public async Task GetSingleIngredient_AfterCreate_ReturnsOk()
        {
            var created = await CreateIngredientAndRead(UniqueName());
            var id = created.GetProperty("id").GetInt32();

            var response = await Client.GetAsync($"/api/v1/ingredients/{id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var jsonDoc = await ReadJson(response);
            Assert.Equal(id, jsonDoc.RootElement.GetProperty("id").GetInt32());

            await DeleteIngredientIfExists(id);
        }

        [Fact]
        public async Task UpdateIngredient_WithPartialPayload_UpdatesOnlyProvidedFields()
        {
            var created = await CreateIngredientAndRead(UniqueName());
            var id = created.GetProperty("id").GetInt32();
            var originalName = created.GetProperty("name").GetString();

            var response = await Client.PutAsync(
                $"/api/v1/ingredients/{id}",
                JsonContent(new { unit = "tbsp" }));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var jsonDoc = await ReadJson(response);
            var ingredient = jsonDoc.RootElement;
            Assert.Equal(originalName, ingredient.GetProperty("name").GetString());
            Assert.Equal("tbsp", ingredient.GetProperty("unit").GetString());

            await DeleteIngredientIfExists(id);
        }

        [Fact]
        public async Task DeleteIngredient_RemovesIngredient()
        {
            var created = await CreateIngredientAndRead(UniqueName());
            var id = created.GetProperty("id").GetInt32();

            var deleteResponse = await Client.DeleteAsync($"/api/v1/ingredients/{id}");
            var getResponse = await Client.GetAsync($"/api/v1/ingredients/{id}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task SearchIngredients_ReturnsCaseInsensitiveMatches()
        {
            var name = UniqueName("CaseSearch");
            var created = await CreateIngredientAndRead(name);
            var id = created.GetProperty("id").GetInt32();

            var response = await Client.GetAsync($"/api/v1/ingredients/search?name={Uri.EscapeDataString(name.ToUpperInvariant())}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var jsonDoc = await ReadJson(response);
            var matches = jsonDoc.RootElement.EnumerateArray().ToList();
            Assert.Contains(matches, ingredient => ingredient.GetProperty("id").GetInt32() == id);

            await DeleteIngredientIfExists(id);
        }

        [Fact]
        public async Task CreateIngredient_WithDuplicateName_ReturnsConflict()
        {
            var name = UniqueName("Duplicate");
            var created = await CreateIngredientAndRead(name);
            var id = created.GetProperty("id").GetInt32();

            var duplicateResponse = await CreateIngredient(name);

            Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);

            await DeleteIngredientIfExists(id);
        }

        private async Task<HttpResponseMessage> CreateIngredient(string name)
        {
            return await Client.PostAsync("/api/v1/ingredients", JsonContent(new
            {
                name,
                quantity = 1,
                unit = "g",
                caloriesPerUnit = 1.25m,
                protein = 0.5m,
                carbs = 0.75m,
                fat = 0.25m
            }));
        }

        private async Task<JsonElement> CreateIngredientAndRead(string name)
        {
            var response = await CreateIngredient(name);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            using var jsonDoc = await ReadJson(response);
            return jsonDoc.RootElement.Clone();
        }

        private async Task DeleteIngredientIfExists(int id)
        {
            var response = await Client.DeleteAsync($"/api/v1/ingredients/{id}");
            response.Dispose();
        }

        private static StringContent JsonContent(object payload)
        {
            return new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");
        }

        private static async Task<JsonDocument> ReadJson(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(responseBody);
        }

        private static async Task AssertJsonArray(HttpResponseMessage response)
        {
            using var jsonDoc = await ReadJson(response);
            Assert.Equal(JsonValueKind.Array, jsonDoc.RootElement.ValueKind);
        }

        private static string UniqueName(string prefix = "E2EIngredient")
        {
            return $"{prefix}-{Guid.NewGuid():N}";
        }
    }
}
