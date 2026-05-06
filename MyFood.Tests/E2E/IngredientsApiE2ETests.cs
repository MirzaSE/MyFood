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
        public async Task CreateIngredient_AndFetchById_ReturnsCreatedAndOk()
        {
            var token = await RegisterAndLogin("ingredientuser1", "Test@123");
            SetAuthorizationToken(token);

            var payload = JsonSerializer.Serialize(new
            {
                name = "Rolled Oats",
                unit = "g",
                caloriesPerUnit = 3.89m,
                protein = 0.16m,
                carbs = 0.66m,
                fat = 0.07m
            });

            var createResponse = await Client.PostAsync("/api/v1/ingredients", new StringContent(payload, Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdJson = await createResponse.Content.ReadAsStringAsync();
            using var createdDoc = JsonDocument.Parse(createdJson);
            var id = createdDoc.RootElement.GetProperty("id").GetInt32();

            var getResponse = await Client.GetAsync($"/api/v1/ingredients/{id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        }

        [Fact]
        public async Task SearchIngredients_ReturnsOk()
        {
            var token = await RegisterAndLogin("ingredientuser2", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients/search?name=oat");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteIngredient_NotFound_ReturnsNotFound()
        {
            var token = await RegisterAndLogin("ingredientuser3", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.DeleteAsync("/api/v1/ingredients/999999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetIngredientById_NegativeId_ReturnsBadRequest()
        {
            var token = await RegisterAndLogin("ingredientuser4", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients/-1");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
