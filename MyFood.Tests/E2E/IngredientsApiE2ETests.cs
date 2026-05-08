using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

public class IngredientsApiE2ETests
{
    private readonly HttpClient _client;

    public IngredientsApiE2ETests()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://localhost:7124");
    }

    [Fact]
    public async Task GetIngredients_ReturnsOk()
    {
        var response = await _client
            .GetAsync("/api/v1/Ingredient");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetIngredientById_ReturnsResponse()
    {
        var response = await _client
            .GetAsync("/api/v1/Ingredient/1");

        response.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateIngredient_ReturnsResponse()
    {
        var ingredient = new
        {
            name = "Test Rice"
        };

        var response = await _client
            .PostAsJsonAsync(
                "/api/v1/Ingredient",
                ingredient);

        response.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteIngredient_ReturnsResponse()
    {
        var response = await _client
            .DeleteAsync("/api/v1/Ingredient/1");

        response.Should().NotBeNull();
    }
}