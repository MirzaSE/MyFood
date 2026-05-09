using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using FluentAssertions;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        private const string BaseUrl = "/api/v1/ingredient";

        [Fact]
        public async Task GetAllIngredients_WithoutAuthentication_ReturnsOk()
        {
            // Act - GET is AllowAnonymous
            var response = await Client.GetAsync(BaseUrl);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAllIngredients_WithAuthentication_ReturnsOkWithIngredients()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientuser1", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync(BaseUrl);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var ingredients = jsonDoc.RootElement;
            Assert.True(ingredients.ValueKind == JsonValueKind.Array);
        }

        [Fact]
        public async Task GetIngredientById_WithValidId_ReturnsOk()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientuser2", "Test@123");
            SetAuthorizationToken(token);

            // First create an ingredient to get a valid ID
            var createDto = new { name = "Test Ingredient GetById", quantity = 100, foodId = 1 };
            var createContent = new StringContent(
                JsonSerializer.Serialize(createDto),
                Encoding.UTF8,
                "application/json");
            var createResponse = await Client.PostAsync(BaseUrl, createContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();
            using var createDoc = JsonDocument.Parse(createBody);
            var createdId = createDoc.RootElement.GetProperty("id").GetInt32();

            // Act
            var response = await Client.GetAsync($"{BaseUrl}/{createdId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var ingredient = jsonDoc.RootElement;
            Assert.Equal("Test Ingredient GetById", ingredient.GetProperty("name").GetString());

            // Cleanup
            await Client.DeleteAsync($"{BaseUrl}/{createdId}");
        }

        [Fact]
        public async Task GetIngredientById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientuser3", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync($"{BaseUrl}/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_WithValidData_ReturnsCreated()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientuser4", "Test@123");
            SetAuthorizationToken(token);

            var createDto = new { name = "New E2E Ingredient", quantity = 50, foodId = 1 };
            var content = new StringContent(
                JsonSerializer.Serialize(createDto),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await Client.PostAsync(BaseUrl, content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var created = jsonDoc.RootElement;
            Assert.Equal("New E2E Ingredient", created.GetProperty("name").GetString());
            Assert.Equal(50, created.GetProperty("quantity").GetInt32());

            // Cleanup
            var createdId = created.GetProperty("id").GetInt32();
            await Client.DeleteAsync($"{BaseUrl}/{createdId}");
        }

        [Fact]
        public async Task CreateIngredient_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            ClearAuthorizationToken();
            var createDto = new { name = "Test", quantity = 50, foodId = 1 };
            var content = new StringContent(
                JsonSerializer.Serialize(createDto),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await Client.PostAsync(BaseUrl, content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_WithEmptyName_ReturnsBadRequest()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientuser5", "Test@123");
            SetAuthorizationToken(token);

            var createDto = new { name = "", quantity = 50, foodId = 1 };
            var content = new StringContent(
                JsonSerializer.Serialize(createDto),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await Client.PostAsync(BaseUrl, content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateIngredient_WithValidData_ReturnsOk()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientuser6", "Test@123");
            SetAuthorizationToken(token);

            // First create an ingredient
            var createDto = new { name = "Original Name", quantity = 100, foodId = 1 };
            var createContent = new StringContent(
                JsonSerializer.Serialize(createDto),
                Encoding.UTF8,
                "application/json");
            var createResponse = await Client.PostAsync(BaseUrl, createContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();
            using var createDoc = JsonDocument.Parse(createBody);
            var createdId = createDoc.RootElement.GetProperty("id").GetInt32();

            var updateDto = new { name = "Updated Name", quantity = 200 };
            var updateContent = new StringContent(
                JsonSerializer.Serialize(updateDto),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await Client.PutAsync($"{BaseUrl}/{createdId}", updateContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var updated = jsonDoc.RootElement;
            Assert.Equal("Updated Name", updated.GetProperty("name").GetString());
            Assert.Equal(200, updated.GetProperty("quantity").GetInt32());

            // Cleanup
            await Client.DeleteAsync($"{BaseUrl}/{createdId}");
        }

        [Fact]
        public async Task UpdateIngredient_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientuser7", "Test@123");
            SetAuthorizationToken(token);

            var updateDto = new { name = "Updated Name" };
            var updateContent = new StringContent(
                JsonSerializer.Serialize(updateDto),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await Client.PutAsync($"{BaseUrl}/99999", updateContent);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteIngredient_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientuser8", "Test@123");
            SetAuthorizationToken(token);

            // First create an ingredient
            var createDto = new { name = "To Be Deleted", quantity = 100, foodId = 1 };
            var createContent = new StringContent(
                JsonSerializer.Serialize(createDto),
                Encoding.UTF8,
                "application/json");
            var createResponse = await Client.PostAsync(BaseUrl, createContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();
            using var createDoc = JsonDocument.Parse(createBody);
            var createdId = createDoc.RootElement.GetProperty("id").GetInt32();

            // Act
            var response = await Client.DeleteAsync($"{BaseUrl}/{createdId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify deletion
            var getResponse = await Client.GetAsync($"{BaseUrl}/{createdId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteIngredient_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientuser9", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.DeleteAsync($"{BaseUrl}/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteIngredient_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            ClearAuthorizationToken();

            // Act
            var response = await Client.DeleteAsync($"{BaseUrl}/1");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}