using System.Net;
using System.Text;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        [Fact]
        public async Task GetAllIngredients_WithoutAuthentication_ReturnsUnauthorized()
        {
            var response = await Client.GetAsync("/api/v1/ingredients");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllIngredients_WithAuthentication_ReturnsOk()
        {
            var token = await RegisterAndLogin("ingredientuser1", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_WithAuthentication_ReturnsCreated()
        {
            var token = await RegisterAndLogin("ingredientuser2", "Test@123");
            SetAuthorizationToken(token);

            var model = new
            {
                name = $"Salt-{Guid.NewGuid():N}",
                foodEntityId = 1
            };

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("/api/v1/ingredients", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetSingleIngredient_WithInvalidId_ReturnsNotFound()
        {
            var token = await RegisterAndLogin("ingredientuser3", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchIngredients_ReturnsOk()
        {
            var token = await RegisterAndLogin("ingredientuser4", "Test@123");
            SetAuthorizationToken(token);

            var createModel = new
            {
                name = $"Sugar-{Guid.NewGuid():N}",
                foodEntityId = 1
            };

            var createContent = new StringContent(JsonSerializer.Serialize(createModel), Encoding.UTF8, "application/json");
            await Client.PostAsync("/api/v1/ingredients", createContent);

            var response = await Client.GetAsync("/api/v1/ingredients/search?name=Sugar");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateIngredient_WithAuthentication_ReturnsOk()
        {
            var token = await RegisterAndLogin("ingredientuser5", "Test@123");
            SetAuthorizationToken(token);

            var createModel = new
            {
                name = $"Pepper-{Guid.NewGuid():N}",
                foodEntityId = 1
            };

            var createContent = new StringContent(JsonSerializer.Serialize(createModel), Encoding.UTF8, "application/json");
            var createResponse = await Client.PostAsync("/api/v1/ingredients", createContent);
            var createdBody = await createResponse.Content.ReadAsStringAsync();
            using var createdJson = JsonDocument.Parse(createdBody);
            var id = createdJson.RootElement.GetProperty("id").GetInt32();

            var updateModel = new
            {
                name = $"Updated-Pepper-{Guid.NewGuid():N}",
                foodEntityId = 1
            };

            var updateContent = new StringContent(JsonSerializer.Serialize(updateModel), Encoding.UTF8, "application/json");
            var updateResponse = await Client.PutAsync($"/api/v1/ingredients/{id}", updateContent);

            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteIngredient_WithAuthentication_ReturnsNoContent()
        {
            var token = await RegisterAndLogin("ingredientuser6", "Test@123");
            SetAuthorizationToken(token);

            var createModel = new
            {
                name = $"Delete-Me-{Guid.NewGuid():N}",
                foodEntityId = 1
            };

            var createContent = new StringContent(JsonSerializer.Serialize(createModel), Encoding.UTF8, "application/json");
            var createResponse = await Client.PostAsync("/api/v1/ingredients", createContent);
            var createdBody = await createResponse.Content.ReadAsStringAsync();
            using var createdJson = JsonDocument.Parse(createdBody);
            var id = createdJson.RootElement.GetProperty("id").GetInt32();

            var deleteResponse = await Client.DeleteAsync($"/api/v1/ingredients/{id}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }
    }
}