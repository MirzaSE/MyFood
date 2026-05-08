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
            var response = await Client.GetAsync("/api/v1/ingredients?page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllIngredients_WithAuthentication_ReturnsOk()
        {
            var token = await RegisterAndLogin("ingredientuser1", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients?page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"), "Should contain X-Pagination header");
        }

        [Fact]
        public async Task CreateAndGetIngredient_WithAuthentication_ReturnsCreatedAndOk()
        {
            var token = await RegisterAndLogin("ingredientuser2", "Test@123");
            SetAuthorizationToken(token);

            var createModel = new
            {
                name = "Tomato",
                unit = "g",
                caloriesPerUnit = 0.18m,
                protein = 0.01m,
                carbs = 0.04m,
                fat = 0.00m
            };

            var content = new StringContent(
                JsonSerializer.Serialize(createModel),
                Encoding.UTF8,
                "application/json");

            var createResponse = await Client.PostAsync("/api/v1/ingredients", content);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createBody = await createResponse.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(createBody);
            var created = jsonDoc.RootElement;

            var id = created.GetProperty("id").GetInt32();

            var getResponse = await Client.GetAsync($"/api/v1/ingredients/{id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        }

        [Fact]
        public async Task SearchIngredients_WithAuthentication_ReturnsOk()
        {
            var token = await RegisterAndLogin("ingredientuser3", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients/search?name=to&page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"), "Should contain X-Pagination header");
        }

        [Fact]
        public async Task DeleteIngredient_WithAuthentication_ReturnsNoContentOrNotFound()
        {
            var token = await RegisterAndLogin("ingredientuser4", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.DeleteAsync("/api/v1/ingredients/999999");

            Assert.True(
                response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound,
                $"Expected NoContent or NotFound, got {response.StatusCode}");
        }
    }
}
