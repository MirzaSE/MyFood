using System.Net;
using System.Text;
using System.Text.Json;

namespace MyFood.Tests.E2E
{
    [Trait("Category", "E2E")]
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        [Fact]
        public async Task GetAllIngredients_ReturnsOk()
        {
            var response = await Client.GetAsync("/api/v1/ingredients");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_WithValidData_ReturnsCreated()
        {
            var model = new
            {
                name = $"Chicken_{Guid.NewGuid():N}",
                unit = "g",
                caloriesPerUnit = 1.65m,
                protein = 31m,
                carbs = 0m,
                fat = 3.6m
            };

            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");

            var response = await Client.PostAsync("/api/v1/ingredients", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Contains(model.name, responseBody);
            Assert.Contains("g", responseBody);
        }

        [Fact]
        public async Task GetIngredientById_WithExistingIngredient_ReturnsOk()
        {
            var model = new
            {
                name = $"Rice_{Guid.NewGuid():N}",
                unit = "g",
                caloriesPerUnit = 1.30m,
                protein = 2.7m,
                carbs = 28m,
                fat = 0.3m
            };

            var createContent = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");

            var createResponse = await Client.PostAsync("/api/v1/ingredients", createContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(createBody);
            var id = jsonDoc.RootElement.GetProperty("id").GetInt32();

            var getResponse = await Client.GetAsync($"/api/v1/ingredients/{id}");
            var getBody = await getResponse.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.Contains(model.name, getBody);
        }

        [Fact]
        public async Task UpdateIngredient_WithValidData_ReturnsOk()
        {
            var model = new
            {
                name = $"Apple_{Guid.NewGuid():N}",
                unit = "g",
                caloriesPerUnit = 0.52m,
                protein = 0.3m,
                carbs = 14m,
                fat = 0.2m
            };

            var createContent = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");

            var createResponse = await Client.PostAsync("/api/v1/ingredients", createContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(createBody);
            var id = jsonDoc.RootElement.GetProperty("id").GetInt32();

            var updateModel = new
            {
                name = $"GreenApple_{Guid.NewGuid():N}",
                unit = "piece",
                caloriesPerUnit = 80m,
                protein = 0.4m,
                carbs = 20m,
                fat = 0.2m
            };

            var updateContent = new StringContent(
                JsonSerializer.Serialize(updateModel),
                Encoding.UTF8,
                "application/json");

            var updateResponse = await Client.PutAsync($"/api/v1/ingredients/{id}", updateContent);
            var updateBody = await updateResponse.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            Assert.Contains(updateModel.name, updateBody);
            Assert.Contains("piece", updateBody);
        }

        [Fact]
        public async Task DeleteIngredient_WithExistingIngredient_ReturnsNoContent()
        {
            var model = new
            {
                name = $"Milk_{Guid.NewGuid():N}",
                unit = "ml",
                caloriesPerUnit = 0.42m,
                protein = 3.4m,
                carbs = 5m,
                fat = 1m
            };

            var createContent = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");

            var createResponse = await Client.PostAsync("/api/v1/ingredients", createContent);
            var createBody = await createResponse.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(createBody);
            var id = jsonDoc.RootElement.GetProperty("id").GetInt32();

            var deleteResponse = await Client.DeleteAsync($"/api/v1/ingredients/{id}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task SearchIngredients_WithMatchingName_ReturnsOkWithMatch()
        {
            var uniqueName = $"Tomato_{Guid.NewGuid():N}";

            var model = new
            {
                name = uniqueName,
                unit = "g",
                caloriesPerUnit = 0.18m,
                protein = 0.9m,
                carbs = 3.9m,
                fat = 0.2m
            };

            var createContent = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");

            await Client.PostAsync("/api/v1/ingredients", createContent);

            var response = await Client.GetAsync($"/api/v1/ingredients?search={uniqueName}");
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains(uniqueName, body);
        }

        [Fact]
        public async Task CreateIngredient_WithDuplicateName_ReturnsConflict()
        {
            var duplicateName = $"Duplicate_{Guid.NewGuid():N}";

            var model = new
            {
                name = duplicateName,
                unit = "g",
                caloriesPerUnit = 1m,
                protein = 1m,
                carbs = 1m,
                fat = 1m
            };

            var firstContent = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");

            var secondContent = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");

            var firstResponse = await Client.PostAsync("/api/v1/ingredients", firstContent);
            var secondResponse = await Client.PostAsync("/api/v1/ingredients", secondContent);

            Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
            Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        }
    }
}