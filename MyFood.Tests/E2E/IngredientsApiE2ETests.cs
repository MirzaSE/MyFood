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

            var response = await Client.GetAsync("/api/v1/ingredients?page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"));
        }

        [Fact]
        public async Task CreateIngredient_ReturnsCreated()
        {
            var token = await RegisterAndLogin("ingredientuser2", "Test@123");
            SetAuthorizationToken(token);

            var payload = new
            {
                name = "Cucumber",
                unit = "100g",
                caloriesPerUnit = 16,
                protein = 0.7,
                carbs = 3.6,
                fat = 0.1
            };

            var response = await Client.PostAsync(
                "/api/v1/ingredients",
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateDuplicateIngredient_ReturnsConflict()
        {
            var token = await RegisterAndLogin("ingredientuser3", "Test@123");
            SetAuthorizationToken(token);

            var payload = JsonSerializer.Serialize(new
            {
                name = "Spinach",
                unit = "100g",
                caloriesPerUnit = 23,
                protein = 2.9,
                carbs = 3.6,
                fat = 0.4
            });

            await Client.PostAsync("/api/v1/ingredients", new StringContent(payload, Encoding.UTF8, "application/json"));
            var secondResponse = await Client.PostAsync("/api/v1/ingredients", new StringContent(payload, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        }

        [Fact]
        public async Task SearchIngredients_ReturnsMatches()
        {
            var token = await RegisterAndLogin("ingredientuser4", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients?query=rice&page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(body);
            var items = jsonDoc.RootElement.GetProperty("value");
            Assert.True(items.GetArrayLength() > 0);
        }

        [Fact]
        public async Task DeleteIngredient_RemovesIt()
        {
            var token = await RegisterAndLogin("ingredientuser5", "Test@123");
            SetAuthorizationToken(token);

            var createResponse = await Client.PostAsync(
                "/api/v1/ingredients",
                new StringContent(JsonSerializer.Serialize(new
                {
                    name = "Celery",
                    unit = "100g",
                    caloriesPerUnit = 14,
                    protein = 0.7,
                    carbs = 3.0,
                    fat = 0.2
                }), Encoding.UTF8, "application/json"));

            var body = await createResponse.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(body);
            var id = jsonDoc.RootElement.GetProperty("id").GetInt32();

            var deleteResponse = await Client.DeleteAsync($"/api/v1/ingredients/{id}");
            var getResponse = await Client.GetAsync($"/api/v1/ingredients/{id}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
