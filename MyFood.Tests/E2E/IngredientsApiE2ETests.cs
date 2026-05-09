using System.Net;
using System.Text.Json;
using System.Net.Http.Headers;
using MyFood.Api;
using MyFood.Tests.E2E;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MyFood.Tests.E2E
{
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        private const string IngredientsEndpoint = "/api/v1/ingredients";

        [Fact]
        public async Task GetAllIngredients_ReturnsOk()
        {
            // Arrange
            var token = await RegisterAndLogin();
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync(IngredientsEndpoint);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(content);
        }

        [Fact]
        public async Task GetIngredientsUnauthorized_ReturnsForbidden()
        {
            // Arrange
            ClearAuthorizationToken();

            // Act
            var response = await Client.GetAsync(IngredientsEndpoint);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_WithValidData_ReturnsCreated()
        {
            // Arrange
            var token = await RegisterAndLogin();
            SetAuthorizationToken(token);

            var ingredient = new
            {
                name = "Tomato",
                unit = "g",
                caloriesPerUnit = 0.18m,
                protein = 0.08m,
                carbs = 0.03m,
                fat = 0.02m
            };

            var content = new StringContent(
                JsonSerializer.Serialize(ingredient),
                new MediaTypeHeaderValue("application/json"));

            // Act
            var response = await Client.PostAsync(IngredientsEndpoint, content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetIngredientById_WithValidId_ReturnsOk()
        {
            // Arrange
            var token = await RegisterAndLogin();
            SetAuthorizationToken(token);

            // First create an ingredient
            var createIngredient = new
            {
                name = "Onion",
                unit = "g",
                caloriesPerUnit = 0.4m,
                protein = 0.1m,
                carbs = 0.09m,
                fat = 0.1m
            };

            var createContent = new StringContent(
                JsonSerializer.Serialize(createIngredient),
                new MediaTypeHeaderValue("application/json"));

            var createResponse = await Client.PostAsync(IngredientsEndpoint, createContent);
            var responseBody = await createResponse.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var ingredientId = jsonDoc.RootElement.GetProperty("id").GetInt32();

            // Act
            var response = await Client.GetAsync($"{IngredientsEndpoint}/{ingredientId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateIngredient_WithValidData_ReturnsOk()
        {
            // Arrange
            var token = await RegisterAndLogin();
            SetAuthorizationToken(token);

            // Create ingredient
            var createIngredient = new
            {
                name = "Garlic",
                unit = "g",
                caloriesPerUnit = 1.4m,
                protein = 0.06m,
                carbs = 0.2m,
                fat = 0.5m
            };

            var createContent = new StringContent(
                JsonSerializer.Serialize(createIngredient),
                new MediaTypeHeaderValue("application/json"));

            var createResponse = await Client.PostAsync(IngredientsEndpoint, createContent);
            var responseBody = await createResponse.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var ingredientId = jsonDoc.RootElement.GetProperty("id").GetInt32();

            // Update ingredient
            var updateIngredient = new
            {
                name = "Fresh Garlic",
                unit = "cloves"
            };

            var updateContent = new StringContent(
                JsonSerializer.Serialize(updateIngredient),
                new MediaTypeHeaderValue("application/json"));

            // Act
            var response = await Client.PutAsync($"{IngredientsEndpoint}/{ingredientId}", updateContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteIngredient_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var token = await RegisterAndLogin();
            SetAuthorizationToken(token);

            // Create ingredient
            var createIngredient = new
            {
                name = "Pepper",
                unit = "g",
                caloriesPerUnit = 3.2m,
                protein = 0.1m,
                carbs = 0.6m,
                fat = 0.3m
            };

            var createContent = new StringContent(
                JsonSerializer.Serialize(createIngredient),
                new MediaTypeHeaderValue("application/json"));

            var createResponse = await Client.PostAsync(IngredientsEndpoint, createContent);
            var responseBody = await createResponse.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var ingredientId = jsonDoc.RootElement.GetProperty("id").GetInt32();

            // Act
            var response = await Client.DeleteAsync($"{IngredientsEndpoint}/{ingredientId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task SearchIngredients_WithValidTerm_ReturnsMatches()
        {
            // Arrange
            var token = await RegisterAndLogin();
            SetAuthorizationToken(token);

            // Create some ingredients
            var ingredients = new[]
            {
                new { name = "Tomato", unit = "g", caloriesPerUnit = 0.18m, protein = 0.08m, carbs = 0.03m, fat = 0.02m },
                new { name = "Cherry Tomato", unit = "g", caloriesPerUnit = 0.18m, protein = 0.08m, carbs = 0.03m, fat = 0.02m }
            };

            foreach (var ingredient in ingredients)
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(ingredient),
                    new MediaTypeHeaderValue("application/json"));
                await Client.PostAsync(IngredientsEndpoint, content);
            }

            // Act
            var response = await Client.GetAsync($"{IngredientsEndpoint}/search?term=tomato");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseBody);
        }
    }
}
