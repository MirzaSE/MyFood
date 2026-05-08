using MyFood.Application.Dtos;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace MyFood.Tests.E2E
{
    // Inherit from ApiE2ETestBase to get the pre-configured HttpClient (Client)
    public class IngredientsApiE2ETests : ApiE2ETestBase
    {
        [Fact]
        public async Task GetIngredients_ReturnsSuccessAndData()
        {
            var response = await Client.GetAsync("/api/v1/ingredients");
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<IEnumerable<IngredientDto>>();
            Assert.NotNull(data);
        }

        [Fact]
        public async Task GetIngredientById_ReturnsNotFound_ForInvalidId()
        {
            var response = await Client.GetAsync("/api/v1/ingredients/9999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateIngredient_ReturnsCreated_WithValidData()
        {
            var newIngredient = new IngredientCreateDto
            {
                Name = "E2E Test Ingredient"
            };

            var response = await Client.PostAsJsonAsync("/api/v1/ingredients", newIngredient);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            var created = await response.Content.ReadFromJsonAsync<IngredientDto>();
            Assert.NotNull(created);
            Assert.Equal("E2E Test Ingredient", created.Name);
        }

        [Fact]
        public async Task UpdateIngredient_ReturnsOk_WhenValid()
        {
            // First Create
            var newIngredient = new IngredientCreateDto { Name = "To Update" };
            var postResponse = await Client.PostAsJsonAsync("/api/v1/ingredients", newIngredient);
            var created = await postResponse.Content.ReadFromJsonAsync<IngredientDto>();

            // Then Update
            var updateDto = new IngredientUpdateDto { Name = "Updated Name" };
            var putResponse = await Client.PutAsJsonAsync($"/api/v1/ingredients/{created.Id}", updateDto);
            
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteIngredient_ReturnsNoContent()
        {
            // First Create
            var newIngredient = new IngredientCreateDto { Name = "To Delete" };
            var postResponse = await Client.PostAsJsonAsync("/api/v1/ingredients", newIngredient);
            var created = await postResponse.Content.ReadFromJsonAsync<IngredientDto>();

            // Then Delete
            var deleteResponse = await Client.DeleteAsync($"/api/v1/ingredients/{created.Id}");
            
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }
    }
}