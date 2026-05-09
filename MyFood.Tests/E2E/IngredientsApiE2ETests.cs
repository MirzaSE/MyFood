using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        private static StringContent JsonBody(object payload) =>
            new(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        private static string UniqueName(string prefix) => $"{prefix}_{Guid.NewGuid():N}";

        [Fact]
        public async Task GetAllIngredients_WithoutAuthentication_ReturnsUnauthorized()
        {
            var response = await Client.GetAsync("/api/v1/ingredients");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllIngredients_WithAuthentication_ReturnsOkWithEnvelope()
        {
            var token = await RegisterAndLogin("ingredientuser1", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients?page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"));

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            Assert.True(root.TryGetProperty("value", out _));
            Assert.True(root.TryGetProperty("links", out _));
        }

        [Fact]
        public async Task CreateIngredient_WithValidBody_Returns201_AndReturnsDto()
        {
            var token = await RegisterAndLogin("ingredientuser2", "Test@123");
            SetAuthorizationToken(token);

            var name = UniqueName("Apple");
            var payload = new
            {
                name,
                unit = "g",
                caloriesPerUnit = 0.52,
                protein = 0.3,
                carbs = 14.0,
                fat = 0.2
            };

            var response = await Client.PostAsync("/api/v1/ingredients", JsonBody(payload));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            Assert.True(root.TryGetProperty("id", out var id));
            Assert.True(id.GetInt32() > 0);
            Assert.Equal(name, root.GetProperty("name").GetString());
            Assert.Equal("g", root.GetProperty("unit").GetString());
        }

        [Fact]
        public async Task GetSingleIngredient_NotFound_Returns404()
        {
            var token = await RegisterAndLogin("ingredientuser3", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients/999999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateIngredient_PersistsChanges()
        {
            var token = await RegisterAndLogin("ingredientuser4", "Test@123");
            SetAuthorizationToken(token);

            var createResp = await Client.PostAsync("/api/v1/ingredients", JsonBody(new
            {
                name = UniqueName("Banana"),
                unit = "piece",
                caloriesPerUnit = 89.0,
                protein = 1.1,
                carbs = 23.0,
                fat = 0.3
            }));
            createResp.EnsureSuccessStatusCode();
            var createdBody = await createResp.Content.ReadAsStringAsync();
            using var createdDoc = JsonDocument.Parse(createdBody);
            var id = createdDoc.RootElement.GetProperty("id").GetInt32();

            var updatedName = UniqueName("BananaUpdated");
            var updateResp = await Client.PutAsync($"/api/v1/ingredients/{id}", JsonBody(new
            {
                name = updatedName,
                unit = "piece",
                caloriesPerUnit = 95.0,
                protein = 1.2,
                carbs = 24.0,
                fat = 0.4
            }));

            Assert.Equal(HttpStatusCode.OK, updateResp.StatusCode);

            var getResp = await Client.GetAsync($"/api/v1/ingredients/{id}");
            getResp.EnsureSuccessStatusCode();
            var getBody = await getResp.Content.ReadAsStringAsync();
            using var getDoc = JsonDocument.Parse(getBody);
            Assert.Equal(updatedName, getDoc.RootElement.GetProperty("name").GetString());
            Assert.Equal(95.0, getDoc.RootElement.GetProperty("caloriesPerUnit").GetDouble());
        }

        [Fact]
        public async Task DeleteIngredient_Returns204_AndSubsequentGetIs404()
        {
            var token = await RegisterAndLogin("ingredientuser5", "Test@123");
            SetAuthorizationToken(token);

            var createResp = await Client.PostAsync("/api/v1/ingredients", JsonBody(new
            {
                name = UniqueName("Tomato"),
                unit = "g",
                caloriesPerUnit = 0.18,
                protein = 0.9,
                carbs = 3.9,
                fat = 0.2
            }));
            createResp.EnsureSuccessStatusCode();
            var createdBody = await createResp.Content.ReadAsStringAsync();
            using var createdDoc = JsonDocument.Parse(createdBody);
            var id = createdDoc.RootElement.GetProperty("id").GetInt32();

            var deleteResp = await Client.DeleteAsync($"/api/v1/ingredients/{id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);

            var afterResp = await Client.GetAsync($"/api/v1/ingredients/{id}");
            Assert.Equal(HttpStatusCode.NotFound, afterResp.StatusCode);
        }

        [Fact]
        public async Task SearchIngredients_FindsByPartialName()
        {
            var token = await RegisterAndLogin("ingredientuser6", "Test@123");
            SetAuthorizationToken(token);

            var marker = Guid.NewGuid().ToString("N").Substring(0, 8);
            var name = $"Carrot_{marker}";
            await Client.PostAsync("/api/v1/ingredients", JsonBody(new
            {
                name,
                unit = "g",
                caloriesPerUnit = 0.41,
                protein = 0.9,
                carbs = 9.6,
                fat = 0.2
            }));

            var response = await Client.GetAsync($"/api/v1/ingredients/search?name={marker}&page=1&pageCount=10");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            var values = doc.RootElement.GetProperty("value");
            Assert.True(values.GetArrayLength() >= 1);

            bool found = false;
            foreach (var item in values.EnumerateArray())
            {
                if (item.GetProperty("name").GetString() == name)
                {
                    found = true;
                    break;
                }
            }
            Assert.True(found, $"Expected to find ingredient with name {name}");
        }

        [Fact]
        public async Task CreateIngredient_DuplicateName_Returns409()
        {
            var token = await RegisterAndLogin("ingredientuser7", "Test@123");
            SetAuthorizationToken(token);

            var name = UniqueName("DuplicateIngredient");
            var payload = new
            {
                name,
                unit = "g",
                caloriesPerUnit = 1.0,
                protein = 0.0,
                carbs = 0.0,
                fat = 0.0
            };

            var first = await Client.PostAsync("/api/v1/ingredients", JsonBody(payload));
            Assert.Equal(HttpStatusCode.Created, first.StatusCode);

            var second = await Client.PostAsync("/api/v1/ingredients", JsonBody(payload));
            Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        }
    }
}
