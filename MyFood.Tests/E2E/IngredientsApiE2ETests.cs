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

        // 1. Auth required
        [Fact]
        public async Task GetAll_WithoutAuthentication_ReturnsUnauthorized()
        {
            var response = await Client.GetAsync("/api/v1/ingredients");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // 2. Authenticated GET works and returns pagination metadata
        [Fact]
        public async Task GetAll_WithAuthentication_ReturnsOkWithPagination()
        {
            var token = await RegisterAndLogin("ing_user1", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients?page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"));

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.True(doc.RootElement.TryGetProperty("value", out _));
        }

        // 3. Full create -> read -> update -> delete cycle
        [Fact]
        public async Task FullCrudLifecycle_WorksEndToEnd()
        {
            var token = await RegisterAndLogin("ing_user2", "Test@123");
            SetAuthorizationToken(token);

            // Create
            var name = $"Tomato_{Guid.NewGuid():N}";
            var createResp = await Client.PostAsync("/api/v1/ingredients", JsonBody(new
            {
                name,
                unit = "g",
                caloriesPerUnit = 0.18,
                protein = 0.9,
                carbs = 3.9,
                fat = 0.2
            }));
            Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);

            var createdJson = await createResp.Content.ReadAsStringAsync();
            using var createdDoc = JsonDocument.Parse(createdJson);
            var id = createdDoc.RootElement.GetProperty("id").GetInt32();
            Assert.True(id > 0);

            // Read
            var getResp = await Client.GetAsync($"/api/v1/ingredients/{id}");
            Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

            // Update
            var putResp = await Client.PutAsync($"/api/v1/ingredients/{id}", JsonBody(new
            {
                caloriesPerUnit = 0.20
            }));
            Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);

            // Delete
            var delResp = await Client.DeleteAsync($"/api/v1/ingredients/{id}");
            Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);

            // Confirm gone
            var gone = await Client.GetAsync($"/api/v1/ingredients/{id}");
            Assert.Equal(HttpStatusCode.NotFound, gone.StatusCode);
        }

        // 4. Duplicate name returns 409 Conflict
        [Fact]
        public async Task Create_WithDuplicateName_ReturnsConflict()
        {
            var token = await RegisterAndLogin("ing_user3", "Test@123");
            SetAuthorizationToken(token);

            var name = $"Garlic_{Guid.NewGuid():N}";
            var first = await Client.PostAsync("/api/v1/ingredients", JsonBody(new
            {
                name,
                unit = "g",
                caloriesPerUnit = 1.49
            }));
            Assert.Equal(HttpStatusCode.Created, first.StatusCode);

            var second = await Client.PostAsync("/api/v1/ingredients", JsonBody(new
            {
                name,
                unit = "g",
                caloriesPerUnit = 1.49
            }));
            Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        }

        // 5. Validation: missing name -> 400
        [Fact]
        public async Task Create_WithMissingName_ReturnsBadRequest()
        {
            var token = await RegisterAndLogin("ing_user4", "Test@123");
            SetAuthorizationToken(token);

            var resp = await Client.PostAsync("/api/v1/ingredients", JsonBody(new
            {
                unit = "g",
                caloriesPerUnit = 1.0
            }));

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        // 6. GetById with non-existent id -> 404
        [Fact]
        public async Task GetById_WithUnknownId_ReturnsNotFound()
        {
            var token = await RegisterAndLogin("ing_user5", "Test@123");
            SetAuthorizationToken(token);

            var resp = await Client.GetAsync("/api/v1/ingredients/99999");
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        // 7. Search returns 400 when no name supplied
        [Fact]
        public async Task Search_WithEmptyTerm_ReturnsBadRequest()
        {
            var token = await RegisterAndLogin("ing_user6", "Test@123");
            SetAuthorizationToken(token);

            var resp = await Client.GetAsync("/api/v1/ingredients/search?name=");
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        // 8. Search returns ingredients case-insensitively
        [Fact]
        public async Task Search_IsCaseInsensitive()
        {
            var token = await RegisterAndLogin("ing_user7", "Test@123");
            SetAuthorizationToken(token);

            var name = $"Basil_{Guid.NewGuid():N}";
            var create = await Client.PostAsync("/api/v1/ingredients", JsonBody(new
            {
                name,
                unit = "g",
                caloriesPerUnit = 0.23
            }));
            Assert.Equal(HttpStatusCode.Created, create.StatusCode);

            var resp = await Client.GetAsync($"/api/v1/ingredients/search?name={name.ToUpper()[..6]}");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

            var body = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            var values = doc.RootElement.GetProperty("value");
            Assert.True(values.GetArrayLength() >= 1);
        }
    }
}
