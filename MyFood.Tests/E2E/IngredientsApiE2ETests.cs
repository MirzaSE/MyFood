using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace MyFood.Tests.E2E
{
    /// <summary>
    /// Ingredients API is not authorized (see controller). Uses seeded FoodItem id 1 (see SeedDataService).
    /// Runs against in-memory DB (see <see cref="IngredientsWebApplicationFactory"/>).
    /// </summary>
    [Trait("Category", "E2E")]
    public class IngredientsApiE2ETests : IClassFixture<IngredientsWebApplicationFactory>
    {
        private const string BasePath = "/api/v1/ingredients";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _client;

        public IngredientsApiE2ETests(IngredientsWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost:8080/")
            });
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync(BasePath);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var response = await _client.PostAsJsonAsync(BasePath, new { name = $"E2E_Salt_{Guid.NewGuid():N}", foodEntityId = 1 });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound()
        {
            var response = await _client.GetAsync($"{BasePath}/999999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            var create = await _client.PostAsJsonAsync(BasePath,
                new { name = $"E2E_Temp_{Guid.NewGuid():N}", foodEntityId = 1 });

            Assert.Equal(HttpStatusCode.Created, create.StatusCode);

            var item = await create.Content.ReadFromJsonAsync<CreatedIngredientDto>(JsonOptions);
            Assert.NotNull(item);

            var response = await _client.DeleteAsync($"{BasePath}/{item!.Id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Search_ReturnsResults()
        {
            await _client.PostAsJsonAsync(BasePath,
                new { name = $"E2E_Sugar_{Guid.NewGuid():N}", foodEntityId = 1 });

            var response = await _client.GetAsync($"{BasePath}/search?query=sug");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        }

        private sealed class CreatedIngredientDto
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public int FoodEntityId { get; set; }
        }
    }
}
