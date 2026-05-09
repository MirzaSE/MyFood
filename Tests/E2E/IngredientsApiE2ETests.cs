using System.Net;
using System.Text;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        [Fact]
        public async Task GetAllFoods_WithIngredients_ReturnsOk()
        {
            var token = await RegisterAndLogin("inguser1", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/foods?page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateFood_WithIngredients_ReturnsCreated()
        {
            var token = await RegisterAndLogin("inguser2", "Test@123");
            SetAuthorizationToken(token);

            var food = new
            {
                name = "Test Food With Ingredients",
                type = "Test",
                calories = 200
            };

            var content = new StringContent(
                JsonSerializer.Serialize(food),
                Encoding.UTF8,
                "application/json");

            var response = await Client.PostAsync("/api/v1/foods", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetFood_IncludesIngredientsList()
        {
            var token = await RegisterAndLogin("inguser3", "Test@123");
            SetAuthorizationToken(token);

            // Create a food first
            var food = new { name = "Food With Ings", type = "Test", calories = 100 };
            var createContent = new StringContent(JsonSerializer.Serialize(food), Encoding.UTF8, "application/json");
            var createResponse = await Client.PostAsync("/api/v1/foods", createContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();

            using var createDoc = JsonDocument.Parse(createBody);
            var foodId = createDoc.RootElement.GetProperty("id").GetInt32();

            // Get it back
            var response = await Client.GetAsync($"/api/v1/foods/{foodId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateFood_WithoutAuth_ReturnsUnauthorized()
        {
            ClearAuthorizationToken();

            var food = new { name = "Unauthorized Food", type = "Test", calories = 100 };
            var content = new StringContent(JsonSerializer.Serialize(food), Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("/api/v1/foods", content);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteFood_RemovesWithIngredients()
        {
            var token = await RegisterAndLogin("inguser4", "Test@123");
            SetAuthorizationToken(token);

            // Create food
            var food = new { name = "Food To Delete", type = "Test", calories = 50 };
            var createContent = new StringContent(JsonSerializer.Serialize(food), Encoding.UTF8, "application/json");
            var createResponse = await Client.PostAsync("/api/v1/foods", createContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();

            using var createDoc = JsonDocument.Parse(createBody);
            var foodId = createDoc.RootElement.GetProperty("id").GetInt32();

            // Delete it
            var deleteResponse = await Client.DeleteAsync($"/api/v1/foods/{foodId}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }
    }
}