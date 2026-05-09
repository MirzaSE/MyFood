using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.JsonPatch;
using MyFood.Application.Dtos;
using Newtonsoft.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class IngredientServiceTests : ApiE2ETestBase
    {
        private async Task<int> GetExistingFoodIdAsync()
        {
            var response = await Client.GetAsync("/api/v1/foods?page=1&pageCount=1");

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var value = jsonDoc.RootElement.GetProperty("value");

            foreach (var item in value.EnumerateArray())
            {
                return item.GetProperty("id").GetInt32();
            }

            throw new InvalidOperationException("No food items were returned by the API.");
        }

        private async Task<JsonElement> CreateIngredientAsync(string name, int quantity = 1, int? foodEntityId = null)
        {
            var resolvedFoodEntityId = foodEntityId ?? await GetExistingFoodIdAsync();

            var model = new
            {
                Name = name,
                Quantity = quantity,
                FoodEntityId = resolvedFoodEntityId
            };

            var body = new StringContent(System.Text.Json.JsonSerializer.Serialize(model), Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
            var response = await Client.PostAsync("/api/v1/ingredients", body);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            return jsonDoc.RootElement.Clone();
        }

        private async Task<JsonElement> GetIngredientCollectionAsync(string requestUri)
        {
            var response = await Client.GetAsync(requestUri);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            return jsonDoc.RootElement.Clone();
        }

        [Fact]
        public async Task GetAllIngredients_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Act
            var response = await Client.GetAsync("/api/v1/ingredients");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllIngredients_WithAuthentication_ReturnsOkWithIngredients()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientuser1", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v1/ingredients?page=1&pageCount=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Pagination"), "Should contain X-Pagination header");
            
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            
            Assert.True(root.TryGetProperty("value", out _));
            Assert.True(root.TryGetProperty("links", out _));
        }

        [Fact]
        public async Task GetAllIngredients_WithEmptyQuery_ReturnsEmptyResults()
        {
            // Arrange
            var token = await RegisterAndLogin($"ingredientuser-{Guid.NewGuid():N}", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var response = await Client.GetAsync("/api/v1/ingredients?page=9999&pageCount=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            Assert.True(root.TryGetProperty("value", out var value));
            Assert.Equal(0, value.GetArrayLength());
            Assert.True(response.Headers.Contains("X-Pagination"));
        }

        [Fact]
        public async Task GetAllIngredients_Pagination_ReturnsDistinctPages()
        {
            // Arrange
            var token = await RegisterAndLogin($"ingredientuser-{Guid.NewGuid():N}", "Test@123");
            SetAuthorizationToken(token);

            // Act
            var firstPage = await GetIngredientCollectionAsync("/api/v1/ingredients?page=1&pageCount=1");
            var secondPage = await GetIngredientCollectionAsync("/api/v1/ingredients?page=2&pageCount=1");

            // Assert
            Assert.True(firstPage.TryGetProperty("value", out var firstValue));
            Assert.True(secondPage.TryGetProperty("value", out var secondValue));
            Assert.Equal(1, firstValue.GetArrayLength());
            Assert.Equal(1, secondValue.GetArrayLength());

            var firstId = firstValue[0].GetProperty("id").GetInt32();
            var secondId = secondValue[0].GetProperty("id").GetInt32();

            Assert.NotEqual(firstId, secondId);
        }

        [Fact]
        public async Task GetSingle_ByValidId_ReturnsOK()
        {
            var token = await RegisterAndLogin($"ingredientuser-{Guid.NewGuid():N}", "Test@123");
            SetAuthorizationToken(token);

            var createdIngredient = await CreateIngredientAsync($"single-{Guid.NewGuid():N}", 3, 1);
            var ingredientId = createdIngredient.GetProperty("id").GetInt32();

            // Act
            var response = await Client.GetAsync($"/api/v1/ingredients/{ingredientId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            using var ingredientDoc = JsonDocument.Parse(responseBody);
            var ingredient = ingredientDoc.RootElement;
            
            Assert.True(ingredient.TryGetProperty("id", out var id));
            Assert.Equal(ingredientId, id.GetInt32());
            Assert.Equal(createdIngredient.GetProperty("name").GetString(), ingredient.GetProperty("name").GetString());
        }

        [Fact]
        public async Task GetSingle_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientuser1", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients/9999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetSingle_WithNegativeId_ReturnsArgumentOutOfRangeException()
        {
            // Arrange
            var token = await RegisterAndLogin("ingredientuser1", "Test@123");
            SetAuthorizationToken(token);

            var response = await Client.GetAsync("/api/v1/ingredients/-100");

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_WithValidBody_ReturnsCREATED()
        {
            // Arrange
            var token = await RegisterAndLogin($"ingredientuser-{Guid.NewGuid():N}", "Test@123");
            SetAuthorizationToken(token);
            var ingredientName = $"Created-{Guid.NewGuid():N}";
            var model = new {
                Name = ingredientName,
                Quantity = 1,
                FoodEntityId = 1
            };
            var body = new StringContent(System.Text.Json.JsonSerializer.Serialize(model), Encoding.UTF8, new MediaTypeHeaderValue("application/json")); 

            // Act
            var response = await Client.PostAsync("/api/v1/ingredients",
                body
            );

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            Assert.True(root.TryGetProperty("id", out var id));
            Assert.True(id.GetInt32() > 0);
            Assert.Equal(ingredientName, root.GetProperty("name").GetString());
            Assert.Equal(1, root.GetProperty("quantity").GetInt32());
            Assert.Equal(1, root.GetProperty("foodEntityId").GetInt32());
        }

        [Fact]
        public async Task CreateIngredient_WithInvalidBody_ReturnsBADREQUEST()
        {
            // Arrange
            var token = await RegisterAndLogin($"ingredientuser-{Guid.NewGuid():N}", "Test@123");
            SetAuthorizationToken(token);
            var model = "";
            var body = new StringContent(System.Text.Json.JsonSerializer.Serialize(model), Encoding.UTF8, new MediaTypeHeaderValue("application/json")); 

            // Act
            var response = await Client.PostAsync("/api/v1/ingredients",
                body
            );

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_ForNonExistingFood_ReturnsCREATED()
        {
            // Arrange
            var token = await RegisterAndLogin($"ingredientuser-{Guid.NewGuid():N}", "Test@123");
            SetAuthorizationToken(token);
            var model = new {
                Name = $"New Test Ingredient-{Guid.NewGuid():N}",
                Quantity = 1,
                FoodEntityId = 9999
            };
            var body = new StringContent(System.Text.Json.JsonSerializer.Serialize(model), Encoding.UTF8, new MediaTypeHeaderValue("application/json")); 

            // Act
            var response = await Client.PostAsync("/api/v1/ingredients",
                body
            );

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateIngredient_WithValidBody_ReturnsOkAndUpdatesIngredient()
        {
            // Arrange
            var token = await RegisterAndLogin($"ingredientuser-{Guid.NewGuid():N}", "Test@123");
            SetAuthorizationToken(token);

            var createdIngredient = await CreateIngredientAsync($"updatable-{Guid.NewGuid():N}", 1, 1);
            var ingredientId = createdIngredient.GetProperty("id").GetInt32();

            var updatedName = $"Updated-{Guid.NewGuid():N}";
            var updateModel = new
            {
                Name = updatedName,
                Quantity = 7,
                FoodEntityId = 1
            };

            var body = new StringContent(System.Text.Json.JsonSerializer.Serialize(updateModel), Encoding.UTF8, new MediaTypeHeaderValue("application/json"));

            // Act
            var response = await Client.PutAsync($"/api/v1/ingredients/{ingredientId}", body);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            Assert.Equal(ingredientId, root.GetProperty("id").GetInt32());
            Assert.Equal(updatedName, root.GetProperty("name").GetString());
            Assert.Equal(7, root.GetProperty("quantity").GetInt32());
            Assert.Equal(1, root.GetProperty("foodEntityId").GetInt32());
        }

        [Fact]
        public async Task UpdateIngredient_WithMissingIngredient_ReturnsNotFound()
        {
            // Arrange
            var token = await RegisterAndLogin($"ingredientuser-{Guid.NewGuid():N}", "Test@123");
            SetAuthorizationToken(token);

            var updateModel = new
            {
                Name = $"Missing-{Guid.NewGuid():N}",
                Quantity = 2,
                FoodEntityId = 1
            };

            var body = new StringContent(System.Text.Json.JsonSerializer.Serialize(updateModel), Encoding.UTF8, new MediaTypeHeaderValue("application/json"));

            // Act
            var response = await Client.PutAsync("/api/v1/ingredients/9999999", body);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateIngredient_WithPartialBody_ReturnsOkAndChangesOnlyProvidedField()
        {
            // Arrange
            var token = await RegisterAndLogin($"ingredientuser-{Guid.NewGuid():N}", "Test@123");
            SetAuthorizationToken(token);

            var createdIngredient = await CreateIngredientAsync($"partial-{Guid.NewGuid():N}", 5);
            var ingredientId = createdIngredient.GetProperty("id").GetInt32();

            var patchDocument = new JsonPatchDocument<IngredientUpdateDto>();
            patchDocument.Replace(x => x.Name, "Partial-Updated");
            var patchJson = JsonConvert.SerializeObject(patchDocument);
            var patchRequest = new HttpRequestMessage(HttpMethod.Patch, $"/api/v1/ingredients/{ingredientId}")
            {
                Content = new StringContent(patchJson, Encoding.UTF8, "application/json-patch+json")
            };

            // Act
            var response = await Client.SendAsync(patchRequest);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Expected success but got {(int)response.StatusCode} {response.StatusCode}. Body: {responseBody}");
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            Assert.Equal(ingredientId, root.GetProperty("id").GetInt32());
            Assert.Equal("Partial-Updated", root.GetProperty("name").GetString());
            Assert.Equal(5, root.GetProperty("quantity").GetInt32());
            Assert.Equal(2, root.GetProperty("foodEntityId").GetInt32());
        }

        [Fact]
        public async Task DeleteIngredient_WithExistingId_ThenDeletingAgain_ReturnsNoContent_ThenNotFound()
        {
            // Arrange
            var token = await RegisterAndLogin($"ingredientuser-{Guid.NewGuid():N}", "Test@123");
            SetAuthorizationToken(token);

            var createdIngredient = await CreateIngredientAsync($"delete-{Guid.NewGuid():N}", 1, 1);
            var ingredientId = createdIngredient.GetProperty("id").GetInt32();

            // Act 1
            var deleteResponse = await Client.DeleteAsync($"/api/v1/ingredients/{ingredientId}");

            // Assert 1
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Act 2
            var deleteAgainResponse = await Client.DeleteAsync($"/api/v1/ingredients/{ingredientId}");

            // Assert 2
            Assert.Equal(HttpStatusCode.NotFound, deleteAgainResponse.StatusCode);
        }

        [Fact]
        public async Task SearchIngredients_WithExistingTerm_ReturnsMatches()
        {
            // Arrange
            var token = await RegisterAndLogin($"ingredientuser-{Guid.NewGuid():N}", "Test@123");
            SetAuthorizationToken(token);

            var ingredientName = $"Searchable-{Guid.NewGuid():N}";
            var createdIngredient = await CreateIngredientAsync(ingredientName, 1, 1);
            var ingredientId = createdIngredient.GetProperty("id").GetInt32();

            // Act
            var response = await Client.GetAsync($"/api/v1/ingredients/search?name={ingredientName}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            var value = root.GetProperty("value");

            Assert.True(value.GetArrayLength() >= 1);
            Assert.Equal(ingredientId, value[0].GetProperty("id").GetInt32());
        }

        [Fact]
        public async Task SearchIngredients_WithNoMatches_ReturnsEmptyResults()
        {
            // Arrange
            var token = await RegisterAndLogin($"ingredientuser-{Guid.NewGuid():N}", "Test@123");
            SetAuthorizationToken(token);

            var searchTerm = $"no-result-{Guid.NewGuid():N}";

            // Act
            var response = await Client.GetAsync($"/api/v1/ingredients/search?name={searchTerm}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            Assert.Equal(0, root.GetProperty("value").GetArrayLength());
        }

        [Fact]
        public async Task SearchIngredients_IsCaseInsensitive()
        {
            // Arrange
            var token = await RegisterAndLogin($"ingredientuser-{Guid.NewGuid():N}", "Test@123");
            SetAuthorizationToken(token);

            var ingredientName = $"CaseMix-{Guid.NewGuid():N}";
            var createdIngredient = await CreateIngredientAsync(ingredientName, 1, 1);
            var ingredientId = createdIngredient.GetProperty("id").GetInt32();

            // Act
            var lowerResponse = await Client.GetAsync($"/api/v1/ingredients/search?name={ingredientName.ToLowerInvariant()}");
            var upperResponse = await Client.GetAsync($"/api/v1/ingredients/search?name={ingredientName.ToUpperInvariant()}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, lowerResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, upperResponse.StatusCode);

            var lowerBody = await lowerResponse.Content.ReadAsStringAsync();
            var upperBody = await upperResponse.Content.ReadAsStringAsync();

            using var lowerDoc = JsonDocument.Parse(lowerBody);
            using var upperDoc = JsonDocument.Parse(upperBody);

            var lowerValue = lowerDoc.RootElement.GetProperty("value");
            var upperValue = upperDoc.RootElement.GetProperty("value");

            Assert.True(lowerValue.GetArrayLength() >= 1);
            Assert.True(upperValue.GetArrayLength() >= 1);
            Assert.Equal(ingredientId, lowerValue[0].GetProperty("id").GetInt32());
            Assert.Equal(ingredientId, upperValue[0].GetProperty("id").GetInt32());
        }

    }
}