using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        private const string BasePath = "/api/v1/ingredients";

        // ----- 1. Auth gate -----
        [Fact]
        public async Task GetAllIngredients_WithoutAuthentication_ReturnsUnauthorized()
        {
            var response = await Client.GetAsync(BasePath);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ----- 2. List with pagination metadata -----
        [Fact]
        public async Task GetAllIngredients_WithAuthentication_ReturnsOkAndPaginationHeader()
        {
            var token = await RegisterAndLogin("ing_user1", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync($"{BasePath}?page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"), "X-Pagination header should be present");

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.True(doc.RootElement.TryGetProperty("value", out _));
        }

        // ----- 3. Create + Get-by-id round-trip -----
        [Fact]
        public async Task CreateIngredient_ThenGetById_ReturnsTheSameIngredient()
        {
            var token = await RegisterAndLogin("ing_user2", "Test@123");
            SetAuthorizationToken(token);

            var unique = $"e2e_{Guid.NewGuid():N}".Substring(0, 16);
            var payload = new
            {
                name = unique,
                unit = "g",
                caloriesPerUnit = 1.5,
                protein = 0.1,
                carbs = 0.2,
                fat = 0.05
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, new MediaTypeHeaderValue("application/json"));

            var createResponse = await Client.PostAsync(BasePath, content);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdBody = await createResponse.Content.ReadAsStringAsync();
            using var createdDoc = JsonDocument.Parse(createdBody);
            var id = createdDoc.RootElement.GetProperty("id").GetInt32();
            var nameBack = createdDoc.RootElement.GetProperty("name").GetString();
            Assert.Equal(unique, nameBack);

            var getResponse = await Client.GetAsync($"{BasePath}/{id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getBody = await getResponse.Content.ReadAsStringAsync();
            using var getDoc = JsonDocument.Parse(getBody);
            Assert.Equal(unique, getDoc.RootElement.GetProperty("name").GetString());
            Assert.Equal("g",    getDoc.RootElement.GetProperty("unit").GetString());
        }

        // ----- 4. Get-by-id 404 -----
        [Fact]
        public async Task GetIngredient_WithUnknownId_ReturnsNotFound()
        {
            var token = await RegisterAndLogin("ing_user3", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync($"{BasePath}/999999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // ----- 5. Search by name (case-insensitive) -----
        [Fact]
        public async Task SearchIngredients_FindsCreatedItem_CaseInsensitive()
        {
            var token = await RegisterAndLogin("ing_user4", "Test@123");
            SetAuthorizationToken(token);

            var unique = $"E2ESearch_{Guid.NewGuid():N}".Substring(0, 18);
            var payload = new { name = unique, unit = "g", caloriesPerUnit = 0.0, protein = 0.0, carbs = 0.0, fat = 0.0 };
            var content = new StringContent(JsonSerializer.Serialize(payload), new MediaTypeHeaderValue("application/json"));
            var createResponse = await Client.PostAsync(BasePath, content);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            // Search with lowercase query — should still match.
            var search = await Client.GetAsync($"{BasePath}/search?name={unique.ToLower()}");
            Assert.Equal(HttpStatusCode.OK, search.StatusCode);

            var body = await search.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            var values = doc.RootElement.GetProperty("value");
            Assert.True(values.GetArrayLength() >= 1, "Expected at least one search result");
        }

        // ----- 6. Update flow -----
        [Fact]
        public async Task UpdateIngredient_ChangesFields_AndReflectsOnGet()
        {
            var token = await RegisterAndLogin("ing_user5", "Test@123");
            SetAuthorizationToken(token);

            var unique = $"e2eUpd_{Guid.NewGuid():N}".Substring(0, 16);
            var createPayload = new { name = unique, unit = "g", caloriesPerUnit = 1.0, protein = 0.0, carbs = 0.0, fat = 0.0 };
            var create = await Client.PostAsync(BasePath, new StringContent(JsonSerializer.Serialize(createPayload), new MediaTypeHeaderValue("application/json")));
            Assert.Equal(HttpStatusCode.Created, create.StatusCode);

            var createdBody = await create.Content.ReadAsStringAsync();
            using var createdDoc = JsonDocument.Parse(createdBody);
            var id = createdDoc.RootElement.GetProperty("id").GetInt32();

            // Partial update — only caloriesPerUnit.
            var updatePayload = new { caloriesPerUnit = 2.5 };
            var update = await Client.PutAsync($"{BasePath}/{id}", new StringContent(JsonSerializer.Serialize(updatePayload), new MediaTypeHeaderValue("application/json")));
            Assert.Equal(HttpStatusCode.OK, update.StatusCode);

            var get = await Client.GetAsync($"{BasePath}/{id}");
            var getBody = await get.Content.ReadAsStringAsync();
            using var getDoc = JsonDocument.Parse(getBody);
            Assert.Equal(2.5, getDoc.RootElement.GetProperty("caloriesPerUnit").GetDouble());
            // Name should be preserved (partial update).
            Assert.Equal(unique, getDoc.RootElement.GetProperty("name").GetString());
        }

        // ----- 7. Delete flow -----
        [Fact]
        public async Task DeleteIngredient_RemovesIt_SubsequentGetIs404()
        {
            var token = await RegisterAndLogin("ing_user6", "Test@123");
            SetAuthorizationToken(token);

            var unique = $"e2eDel_{Guid.NewGuid():N}".Substring(0, 16);
            var createPayload = new { name = unique, unit = "g", caloriesPerUnit = 1.0, protein = 0.0, carbs = 0.0, fat = 0.0 };
            var create = await Client.PostAsync(BasePath, new StringContent(JsonSerializer.Serialize(createPayload), new MediaTypeHeaderValue("application/json")));
            Assert.Equal(HttpStatusCode.Created, create.StatusCode);

            using var createdDoc = JsonDocument.Parse(await create.Content.ReadAsStringAsync());
            var id = createdDoc.RootElement.GetProperty("id").GetInt32();

            var del = await Client.DeleteAsync($"{BasePath}/{id}");
            Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);

            var get = await Client.GetAsync($"{BasePath}/{id}");
            Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
        }
    }
}
