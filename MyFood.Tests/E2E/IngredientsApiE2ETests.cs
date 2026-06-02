using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MyFood.Api;
using MyFood.Application.Dtos;
using Xunit;

namespace MyFood.Tests.E2E;

public class IngredientsApiE2ETests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public IngredientsApiE2ETests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllIngredients_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/ingredients");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateIngredient_ReturnsCreatedAndAllowsReadback()
    {
        var createDto = new IngredientCreateDto
        {
            Name = "Test Ingredient",
            Unit = "g",
            CaloriesPerUnit = 10,
            Protein = 0.5,
            Carbs = 1.2,
            Fat = 0.3
        };

        var postResponse = await _client.PostAsJsonAsync("/api/v1/ingredients", createDto);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content.ReadFromJsonAsync<IngredientDto>();
        Assert.NotNull(created);
        Assert.Equal("Test Ingredient", created!.Name);

        var getResponse = await _client.GetAsync($"/api/v1/ingredients/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var readBack = await getResponse.Content.ReadFromJsonAsync<IngredientDto>();
        Assert.NotNull(readBack);
        Assert.Equal(created.Id, readBack!.Id);
    }

    [Fact]
    public async Task SearchIngredients_ReturnsMatchingResults()
    {
        var createDto = new IngredientCreateDto
        {
            Name = "Searchable Ingredient",
            Unit = "g",
            CaloriesPerUnit = 5,
            Protein = 0,
            Carbs = 1,
            Fat = 0
        };

        await _client.PostAsJsonAsync("/api/v1/ingredients", createDto);

        var response = await _client.GetAsync("/api/v1/ingredients/search?name=searchable");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IngredientSearchResult>();
        Assert.NotNull(result);
        Assert.Contains(result!.Value, ingredient => ingredient.Name == "Searchable Ingredient");
    }

    [Fact]
    public async Task DeleteIngredient_ReturnsNoContentAndThenNotFound()
    {
        var createDto = new IngredientCreateDto
        {
            Name = "Delete Ingredient",
            Unit = "g",
            CaloriesPerUnit = 3,
            Protein = 0,
            Carbs = 1,
            Fat = 0
        };

        var postResponse = await _client.PostAsJsonAsync("/api/v1/ingredients", createDto);
        var created = await postResponse.Content.ReadFromJsonAsync<IngredientDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/ingredients/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/v1/ingredients/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetIngredientById_ReturnsNotFoundForMissingItem()
    {
        var response = await _client.GetAsync("/api/v1/ingredients/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private class IngredientSearchResult
    {
        public IngredientDto[] Value { get; set; } = Array.Empty<IngredientDto>();
        public object[] Links { get; set; } = Array.Empty<object>();
    }
}
