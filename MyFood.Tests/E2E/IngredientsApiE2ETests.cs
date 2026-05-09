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
            var token = await RegisterAndLogin("inguser1", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients?page=1&pageCount=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"));
        }

        [Fact]
        public async Task CreateIngredient_WithValidData_ReturnsCreated()
        {
            var token = await RegisterAndLogin("inguser2", "Test@123");
            SetAuthorizationToken(token);

            var uniqueName = $"TestSalt_{Guid.NewGuid():N}";
            var body = JsonSerializer.Serialize(new
            {
                name = uniqueName,
                unit = "g",
                caloriesPerUnit = 0.0,
                protein = 0.0,
                carbs = 0.0,
                fat = 0.0,
                quantity = 5
            });

            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("/api/v1/ingredients", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);
            Assert.Equal(uniqueName, doc.RootElement.GetProperty("name").GetString());
        }

        [Fact]
        public async Task GetIngredientById_WithInvalidId_ReturnsNotFound()
        {
            var token = await RegisterAndLogin("inguser3", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteIngredient_WithInvalidId_ReturnsNotFound()
        {
            var token = await RegisterAndLogin("inguser4", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.DeleteAsync("/api/v1/ingredients/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SearchIngredients_WithEmptyTerm_ReturnsBadRequest()
        {
            var token = await RegisterAndLogin("inguser5", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients/search?name=");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_WithMissingName_ReturnsBadRequest()
        {
            var token = await RegisterAndLogin("inguser6", "Test@123");
            SetAuthorizationToken(token);

            var body = JsonSerializer.Serialize(new { unit = "g", quantity = 5 });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("/api/v1/ingredients", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
